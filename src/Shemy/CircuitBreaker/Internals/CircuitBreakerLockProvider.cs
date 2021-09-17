using System.Collections.Concurrent;
using System.Threading;
using Shemy.CircuitBreaker.Abstractions;

namespace Shemy.CircuitBreaker.Internals
{
    public class CircuitBreakerLockProvider : ICircuitBreakerLockProvider
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> SemaphoreSlims = new();


        public CircuitBreakerLockProvider(string name)
        {
            SemaphoreSlims.TryAdd(name, new SemaphoreSlim(1));
        }

        public SemaphoreSlim TrySemaphore(string name)
        {
            SemaphoreSlims.TryGetValue(name, out var semaphore);

            return semaphore;
        }
    }
}