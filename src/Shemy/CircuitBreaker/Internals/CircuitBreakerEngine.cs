using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.CircuitBreaker.Abstractions;
using Shemy.CircuitBreaker.Configurations;
using Shemy.CircuitBreaker.Exceptions;
using Shemy.Extensions;
using Shemy.Locking;
using Shemy.Request;

namespace Shemy.CircuitBreaker.Internals
{
    public class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly IOptionsSnapshot<CircuitBreakerConfigure> _options;
        private readonly ICircuitBreakerStateProvider _circuitBreakerStateProvider;
        private readonly ICircuitBreakerLockProvider _lockProvider;
        private readonly ICircuitBreakerMetric _metric;

        public CircuitBreakerEngine(IOptionsSnapshot<CircuitBreakerConfigure> options,
            ICircuitBreakerLockProvider lockProvider, ICircuitBreakerStateProvider circuitBreakerStateProvider, ICircuitBreakerMetric metric)
        {
            _options = options;
            _lockProvider = lockProvider;
            _circuitBreakerStateProvider = circuitBreakerStateProvider;
            _metric = metric;
        }


        public async Task<HttpResponseMessage> ExecuteAsync(AnshanHttpRequestMessage request,
            Func<Task<HttpResponseMessage>> next)
        {
            if (_circuitBreakerStateProvider.GetState(request.ClientName) == CircuitBreakerState.Closed)
            {
                return await ExecuteIfCircuitIsCloseAsync(next, request.ClientName);
            }

            ThrowExceptionIfTimeoutExpired(request.ClientName);


            return await TryExecuteIfCircuitIsOpenAsync(next, request.ClientName);
        }

        private SemaphoreSlim TrySemaphore(string name)
        {
            lock (name)
            {
                var semaphore = _lockProvider.TrySemaphore(name);
                if (semaphore is not {CurrentCount: 0}) return semaphore;

                _metric.IncrementFailure(name);
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> TryExecuteIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
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

        private void ThrowExceptionIfTimeoutExpired(string name)
        {
            var configure = _options.Get(name);
            var dateTime =
                new DateTime(configure.DurationOfBreak.Ticks + _metric.GetLastFailureDateTimeUtc(name).Ticks);

            if (dateTime >= DateTime.UtcNow)
                throw new CircuitBreakerOpenException("");
        }

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsCloseAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
            var response = await next();

            if (response.IsSucceed())
                return response;

            var configure = _options.Get(name);
            _metric.IncrementFailure(name);

            if (configure.ExceptionsAllowedBeforeBreaking < _metric.GetFailures(name))
                _circuitBreakerStateProvider.Trip(name);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
            // Set the circuit breaker state to HalfOpen.
            _circuitBreakerStateProvider.HalfOpen(name);

            // Attempt the operation.
            var response = await next();

            if (response.IsSucceed())
            {
                // If this action succeeds, reset the state and allow other operations.
                // In reality, instead of immediately returning to the Closed state, a counter
                // here would record the number of successful operations and return the
                // circuit breaker to the Closed state only after a specified number succeed.

                var configure = _options.Get(name);
                _metric.IncrementSuccess(name);

                if (configure.SuccessAllowedBeforeClosing <= _metric.GetSuccess(name))
                    _circuitBreakerStateProvider.Reset(name);

                return response;
            }

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                _circuitBreakerStateProvider.Reset(name);
                return response;
            }

            _metric.IncrementFailure(name);
            _circuitBreakerStateProvider.Trip(name);

            return response;
        }
    }
}