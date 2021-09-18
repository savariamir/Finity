using System.Threading;

namespace Finity.CircuitBreaker.Abstractions
{
    public interface ICircuitBreakerLockProvider
    {
        SemaphoreSlim TrySemaphore(string name);
    }
}