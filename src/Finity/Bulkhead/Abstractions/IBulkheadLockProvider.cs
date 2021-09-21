using System.Threading;

namespace Finity.Bulkhead.Abstractions
{
    public interface IBulkheadLockProvider
    {
        SemaphoreSlim TrySemaphore(string name);
    }
}