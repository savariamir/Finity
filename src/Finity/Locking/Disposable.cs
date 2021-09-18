using System;

namespace Finity.Locking
{
    public class Disposable : IDisposable
    {
        public static Disposable Create(Action onDispose) => new Disposable(onDispose);

        Action _onDispose;
        private Disposable(Action onDispose) => _onDispose = onDispose;

        public void Dispose()
        {
            _onDispose?.Invoke(); // Execute disposal action if non-null.
            _onDispose = null; // Ensure it can’t execute a second time.
        }
    }
}