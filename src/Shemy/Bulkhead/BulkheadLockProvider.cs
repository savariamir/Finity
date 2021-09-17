using System.Collections.Concurrent;
using System.Threading;

namespace Shemy.Bulkhead
{
    public class BulkheadLockProvider : IBulkheadLockProvider
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> SemaphoreSlims = new();

        public BulkheadLockProvider(string name, int maxConcurrentCalls)
        {
            SemaphoreSlims.TryAdd(name, new SemaphoreSlim(maxConcurrentCalls));
        }

        public SemaphoreSlim TrySemaphore(string name)
        {
            SemaphoreSlims.TryGetValue(name, out var semaphore);
            return semaphore;
        }
    }
}