using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.HttpResponse;
using Shemy.Locking;
using Shemy.Request;

namespace Shemy.CircuitBreaker
{
    internal class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly IOptionsSnapshot<CircuitBreakerConfigure> _options;
        private readonly ICircuitBreakerMetric _metric;

        public CircuitBreakerEngine(IOptionsSnapshot<CircuitBreakerConfigure> options, ICircuitBreakerMetric metric)
        {
            _options = options;
            _metric = metric;
        }

        private void Trip(string name)
        {
            _metric.SetState(CircuitBreakerState.Open, name);
        }


        private void Reset(string name)
        {
            _metric.SetState(CircuitBreakerState.Closed, name);
        }

        private void HalfOpen(string name)
        {
            _metric.SetState(CircuitBreakerState.Closed, name);
        }

        public async Task<HttpResponseMessage> ExecuteAsync(AnshanHttpRequestMessage request,
            Func<Task<HttpResponseMessage>> next)
        {
            if (_metric.GetState(request.ClientName) == CircuitBreakerState.Closed)
            {
                return await ExecuteIfCircuitIsCloseAsync(next, request.ClientName);
            }

            ThrowExceptionIfTimeoutExpired(request.ClientName);


            return await TryExecuteIfCircuitIsOpenAsync(next, request.ClientName);
        }

        private void ThrowExceptionIfAnotherRequestHasAlreadyEntered(string name)
        {
            var semaphore = _options.Get(name).SemaphoreSlim;
            lock (name)
            {
                if (semaphore is not {CurrentCount: 0}) return;

                _metric.IncrementFailure(name);
                throw new CircuitBreakerOpenException("");
            }
        }

        private async Task<HttpResponseMessage> TryExecuteIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
            ThrowExceptionIfAnotherRequestHasAlreadyEntered(name);

            var semaphore = _options.Get(name).SemaphoreSlim;
            using (await semaphore.EnterAsync())
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

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsCloseAsync(Func<Task<HttpResponseMessage>> next, string name)
        {
            var response = await next();

            if (response.IsSucceed())
                return response;

            var configure = _options.Get(name);
            _metric.IncrementFailure(name);

            if (configure.ExceptionsAllowedBeforeBreaking < _metric.GetFailures(name))
                Trip(name);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteIfCircuitIsOpenAsync(Func<Task<HttpResponseMessage>> next,
            string name)
        {
            // Set the circuit breaker state to HalfOpen.
            HalfOpen(name);

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
                    Reset(name);

                return response;
            }

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                Reset(name);
                return response;
            }

            _metric.IncrementFailure(name);
            Trip(name);

            return response;
        }
    }
}