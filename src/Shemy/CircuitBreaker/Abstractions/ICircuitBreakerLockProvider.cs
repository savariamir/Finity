using System.Threading;

namespace Shemy.CircuitBreaker.Abstractions
{
    public interface ICircuitBreakerLockProvider
    {
        SemaphoreSlim TrySemaphore(string name);
    }
}