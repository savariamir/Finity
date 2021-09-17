using System.Threading;

namespace Shemy.Bulkhead
{
    public interface IBulkheadLockProvider
    {
        SemaphoreSlim TrySemaphore(string name);
    }
}