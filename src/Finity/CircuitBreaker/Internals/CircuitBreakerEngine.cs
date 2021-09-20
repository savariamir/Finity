using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.CircuitBreaker.Abstractions;
using Finity.CircuitBreaker.Configurations;
using Finity.CircuitBreaker.Exceptions;
using Finity.Extensions;
using Finity.Locking;
using Finity.Metric;
using Finity.Request;
using Finity.Shared;
using Microsoft.Extensions.Options;

namespace Finity.CircuitBreaker.Internals
{
    public class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly IOptionsSnapshot<CircuitBreakerConfigure> _options;
        private readonly ICircuitBreakerStateProvider _circuitBreakerStateProvider;
        private readonly ICircuitBreakerLockProvider _lockProvider;
        private readonly ICircuitBreakerMetric _metric;

        public CircuitBreakerEngine(IOptionsSnapshot<CircuitBreakerConfigure> options,
                                    ICircuitBreakerLockProvider lockProvider,
                                    ICircuitBreakerStateProvider circuitBreakerStateProvider,
                                    ICircuitBreakerMetric metric)
        {
            _options = options;
            _lockProvider = lockProvider;
            _circuitBreakerStateProvider = circuitBreakerStateProvider;
            _metric = metric;
        }


        public async Task<HttpResponseMessage> ExecuteAsync(AnshanHttpRequestMessage request,
                                                            Func<Task<HttpResponseMessage>> next)
        {
            if (_circuitBreakerStateProvider.GetState(request.Name) == CircuitBreakerState.Closed)
            {
                return await ExecuteIfCircuitIsCloseAsync(next, request.Name);
            }

            //Report that circuit is open
            Metrics.Increment(Metrics.CircuitBreakerOpenedCount);

            return await TryExecutingIfCircuitIsOpenAsync(next, request.Name);
        }

        private SemaphoreSlim TrySemaphore(string name)
        {
            lock (name)
            {
                var semaphore = _lockProvider.TrySemaphore(name);
                if (semaphore is not { CurrentCount: 0 })
                {
                    return semaphore;
                }

                _metric.IncrementFailure(name);
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> TryExecutingIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
            VerifyTimeout(name);

            using (await TrySemaphore(name).EnterAsync())
            {
                var response = await ExecuteIfCircuitIsOpenAsync(next, name);

                if (response.IsSucceed())
                {
                    _metric.IncrementSuccess(name);
                    return response;
                }

                _metric.IncrementFailure(name);
                throw new CircuitBreakerOpenException("");
            }
        }

        private void VerifyTimeout(string name)
        {
            var configure = _options.Get(name);
            var dateTime =
                new DateTime(configure.DurationOfBreak.Ticks + _metric.GetLastFailureDateTimeUtc(name).Ticks);

            if (dateTime >= DateTime.UtcNow)
            {
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsCloseAsync(Func<Task<HttpResponseMessage>> next,
                                                                             string name)
        {
            var response = await next();
            if (response.IsSucceed())
            {
                return response;
            }

            var configure = _options.Get(name);
            _metric.IncrementFailure(name);

            if (configure.ExceptionsAllowedBeforeBreaking >= _metric.GetFailures(name)) return response;
            
            
            //report: Circuit is opened
            Metrics.Increment(Metrics.CircuitBreakerClosedCount);
            _circuitBreakerStateProvider.Trip(name);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
                                                                            string name)
        {
            // Set the circuit breaker state to HalfOpen.
            _circuitBreakerStateProvider.HalfOpen(name);

            var response = await next();
            if (response.IsSucceed())
            {
                var configure = _options.Get(name);
                _metric.IncrementSuccess(name);

                if (configure.SuccessAllowedBeforeClosing <= _metric.GetSuccess(name))
                {
                    //circuit is closed
                    _circuitBreakerStateProvider.Reset(name);
                }

                return response;
            }

            _metric.IncrementFailure(name);
            _circuitBreakerStateProvider.Trip(name);

            return response;
        }
    }
}