using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shemy.Locking
{
    public static class SemaphoreExtension
    {
        public static async Task<IDisposable> EnterAsync(this SemaphoreSlim ss)
        {
            await ss.WaitAsync().ConfigureAwait(false);
            return Disposable.Create(() => ss.Release());
        }
    }
}