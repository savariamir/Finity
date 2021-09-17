using Shemy.CircuitBreaker.Internals;

namespace Shemy.CircuitBreaker.Abstractions
{
    public interface ICircuitBreakerStateProvider
    {
        void Trip(string name);
        void Reset(string name);
        void HalfOpen(string name);
        CircuitBreakerState GetState(string name);
    }
}