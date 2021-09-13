using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.CircuitBreaker
{
    internal class CircuitBreakerEngine : ICircuitBreakerEngine
    {
        private readonly object _halfOpenSyncObject = new();
        private CircuitBreakerState State { get; set; }
        private Exception LastException { get;  set; }
        private DateTime LastStateChangedDateUtc { get;  set; }

        private void Trip(Exception ex)
        {
            State = CircuitBreakerState.Open;
        }

        private void Trip()
        {
            State = CircuitBreakerState.Open;
        }

        private void Reset()
        {
            State = CircuitBreakerState.Closed;
        }

        private void HalfOpen()
        {
            State = CircuitBreakerState.HalfOpen;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> next)
        {
            if (State == CircuitBreakerState.Closed)
            {
                var response = await ExecuteIfCloseAsync(next);
                return response;
            }

            EnsureTimeoutHasExpired();
            
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(_halfOpenSyncObject, ref lockTaken);
                if (lockTaken)
                {
                    var response = await ExecuteIfOpenAsync(next);
                    return response;
                  
                }
            }
            catch (Exception ex)
            {
                // If there's still an exception, trip the breaker again immediately.
                Trip(ex);

                // Throw the exception so that the caller knows which exception occurred.
                throw;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_halfOpenSyncObject);
                }
            }

            throw new CircuitBreakerOpenException("");
        }

        private void EnsureTimeoutHasExpired()
        {
            var openToHalfOpenWaitTime = DateTime.UtcNow;
            var dateTime = new DateTime(LastStateChangedDateUtc.Ticks + openToHalfOpenWaitTime.Ticks);

            if (dateTime >= DateTime.UtcNow)
                throw new CircuitBreakerOpenException("");
        }

        private async Task<HttpResponseMessage> ExecuteIfCloseAsync(Func<Task<HttpResponseMessage>> next)
        {
            var response = await next();

            if (response.IsSuccessStatusCode)
                return response;

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                // Don't reattempt a bad request
                return response;
            }

            Trip();

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteIfOpenAsync(Func<Task<HttpResponseMessage>> next)
        {
            // Set the circuit breaker state to HalfOpen.
            HalfOpen();

            // Attempt the operation.
            var response = await next();

            if (response.IsSuccessStatusCode)
            {
                // If this action succeeds, reset the state and allow other operations.
                // In reality, instead of immediately returning to the Closed state, a counter
                // here would record the number of successful operations and return the
                // circuit breaker to the Closed state only after a specified number succeed.

                Reset();
                return response;
            }

            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                Reset();
                return response;
            }

            Trip();

            return response;
        }
    }
    
}