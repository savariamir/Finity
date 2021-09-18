using System.Collections.Concurrent;
using Finity.CircuitBreaker.Abstractions;

namespace Finity.CircuitBreaker.Internals
{
    public class CircuitBreakerStateProvider : ICircuitBreakerStateProvider
    {
        private static readonly ConcurrentDictionary<string, CircuitBreakerState> States = new();

        public CircuitBreakerStateProvider(string name)
        {
            States.TryAdd(name, CircuitBreakerState.Closed);
        }

        public void Trip(string name)
        {
            lock (name)
            {
                var value = States.GetOrAdd(name, key => CircuitBreakerState.Closed);
                States.TryUpdate(name, value, CircuitBreakerState.Open);
            }
        }

        public void Reset(string name)
        {
            lock (name)
            {
                var value = States.GetOrAdd(name, key => CircuitBreakerState.Closed);
                States.TryUpdate(name, value, CircuitBreakerState.Closed);
            }
        }

        public void HalfOpen(string name)
        {
            lock (name)
            {
                var value = States.GetOrAdd(name, key => CircuitBreakerState.Closed);
                States.TryUpdate(name, value, CircuitBreakerState.HalfOpen);
            }
        }

        public CircuitBreakerState GetState(string name)
        {
            lock (name)
            {
                return States.GetOrAdd(name, key => CircuitBreakerState.Closed);
            }
        }
    }
}