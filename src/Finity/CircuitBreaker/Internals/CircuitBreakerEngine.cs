using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.CircuitBreaker.Abstractions;
using Finity.CircuitBreaker.Configurations;
using Finity.CircuitBreaker.Exceptions;
using Finity.Clock;
using Finity.Extensions;
using Finity.Locking;
using Finity.Request;
using Finity.Shared;
using Finity.Shared.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Finity.CircuitBreaker.Internals
{
    public class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly IOptionsSnapshot<CircuitBreakerConfigure> _options;
        private readonly ICircuitBreakerStateProvider _circuitBreakerStateProvider;
        private readonly ICircuitBreakerLockProvider _lockProvider;
        private readonly ICircuitBreakerMetric _metric;
        private readonly IClock _clock;
        private readonly ILogger<CircuitBreakerEngine> _logger;

        public CircuitBreakerEngine(IOptionsSnapshot<CircuitBreakerConfigure> options,
            ICircuitBreakerLockProvider lockProvider,
            ICircuitBreakerStateProvider circuitBreakerStateProvider,
            ICircuitBreakerMetric metric,
            IClock clock,
            ILogger<CircuitBreakerEngine> logger)
        {
            _options = options;
            _lockProvider = lockProvider;
            _circuitBreakerStateProvider = circuitBreakerStateProvider;
            _metric = metric;
            _clock = clock;
            _logger = logger;
        }


        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            Func<Type, Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric)
        {
            if (IsCircuitClosed(request.Name))
            {
                return await PassRequest(next, setMetric, request.Name);
            }

            //Report that circuit is open
            // setMetric(new CounterValue());

            return await TryInOpenCircuit(next, request.Name);
        }

        private bool IsCircuitClosed(string name)
        {
            return _circuitBreakerStateProvider.GetState(name) == CircuitBreakerState.Closed;
        }

        private SemaphoreSlim TrySemaphore(string name)
        {
            lock (name)
            {
                var semaphore = _lockProvider.TrySemaphore(name);
                if (semaphore is not {CurrentCount: 0})
                {
                    return semaphore;
                }

                _metric.IncrementFailure(name);
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> TryInOpenCircuit(
            Func<Type, Task<HttpResponseMessage>> next,
            string name)
        {
            VerifyTimeout(name);

            using (await TrySemaphore(name).EnterAsync())
            {
                var response = await TryHalfOpen(next, name);
                return response;
            }
        }

        private void VerifyTimeout(string name)
        {
            var configure = _options.Get(name);
            var dateTime =
                new DateTime(configure.DurationOfBreak.Ticks + _metric.GetLastFailureDateTimeUtc(name).Ticks);

            if (dateTime >= _clock.UtcNow())
            {
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> PassRequest(
            Func<Type, Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            string name)
        {
            var response = await next(typeof(CircuitBreakerMiddleware));
            if (response.IsSuccessful())
            {
                return response;
            }

            var configure = _options.Get(name);
            _metric.IncrementFailure(name);

            if (configure.ExceptionsAllowedBeforeBreaking >= _metric.GetFailures(name)) return response;


            //report: Circuit is open
            _logger.LogInformation("Circuit got open");
            // setMetric(new CounterValue());
            _circuitBreakerStateProvider.Trip(name);

            return response;
        }

        private async Task<HttpResponseMessage> TryHalfOpen(Func<Type, Task<HttpResponseMessage>> next,
            string name)
        {
            // Set the circuit breaker state to HalfOpen.
            _circuitBreakerStateProvider.HalfOpen(name);

            var response = await next(typeof(CircuitBreakerMiddleware));
            if (response.IsSuccessful())
            {
                var configure = _options.Get(name);
                _metric.IncrementSuccess(name);

                if (configure.SuccessAllowedBeforeClosing > _metric.GetSuccess(name)) return response;

                //circuit is closed
                _logger.LogInformation("Circuit got closed");
                _circuitBreakerStateProvider.Reset(name);

                return response;
            }

            _metric.IncrementFailure(name);
            _circuitBreakerStateProvider.Trip(name);

            throw new CircuitBreakerOpenException("");
        }
    }
}