using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.Request;

namespace Shemy.CircuitBreaker
{
    internal class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly object _halfOpenSyncObject = new();

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
                return await ExecuteIfCloseAsync(next, request.ClientName);
            }

            ThrowOpenExceptionIfTimeoutExpired(request.ClientName);


            return await TryExecuteIfOpenedAsync(next, request.ClientName);
        }

        private async Task<HttpResponseMessage> TryExecuteIfOpenedAsync(Func<Task<HttpResponseMessage>> next,
                                                                        string name)
        {
            var lockTaken = false;

            HttpResponseMessage response = null;
            try
            {
                var timeout = TimeSpan.FromSeconds(5);
                Monitor.TryEnter(_halfOpenSyncObject, timeout, ref lockTaken);
                if (lockTaken)
                {
                    response = await ExecuteIfOpenAsync(next, name);
                    return response;
                }

                // return response;
            }
            catch (Exception ex)
            {
                var configure = _options.Get(name);
                _metric.IncrementFailure(name);
                if (configure.ExceptionsAllowedBeforeBreaking < _metric.GetFailures(name))
                    Trip(name);

                // Throw the exception so that the caller knows which exception occurred.
                // throw new CircuitBreakerOpenException("");

                return response;
            }
            finally
            {
                try
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(_halfOpenSyncObject);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            // if (response is not null && response.IsSuccessStatusCode)
            //     return response;

            // _metric.IncrementFailure(name);
            throw new CircuitBreakerOpenException("");
        }

        private void ThrowOpenExceptionIfTimeoutExpired(string name)
        {
            var configure = _options.Get(name);
            var dateTime =
                new DateTime(configure.DurationOfBreak.Ticks + _metric.GetLastFailureDateTimeUtc(name).Ticks);

            if (dateTime >= DateTime.UtcNow)
                throw new CircuitBreakerOpenException("");
        }

        private async Task<HttpResponseMessage> ExecuteIfCloseAsync(Func<Task<HttpResponseMessage>> next, string name)
        {
            var response = await next();

            if (response.IsSuccessStatusCode)
                return response;

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                // Don't reattempt a bad request
                return response;
            }

            var configure = _options.Get(name);
            _metric.IncrementFailure(name);

            if (configure.ExceptionsAllowedBeforeBreaking < _metric.GetFailures(name))
                Trip(name);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteIfOpenAsync(Func<Task<HttpResponseMessage>> next, string name)
        {
            // Set the circuit breaker state to HalfOpen.
            HalfOpen(name);

            // Attempt the operation.
            var response = await next();

            if (response.IsSuccessStatusCode)
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