using System.Threading;

namespace Finity.Bulkhead
{
    public interface IBulkheadLockProvider
    {
        SemaphoreSlim TrySemaphore(string name);
    }
}