using System;
using System.Threading;

namespace Shemy.CircuitBreaker
{
    public class CircuitBreakerConfigure
    {
        public int ExceptionsAllowedBeforeBreaking { set; get; }
        public int SuccessAllowedBeforeClosing { set; get; }
        public TimeSpan DurationOfBreak { set; get; }
        public SemaphoreSlim SemaphoreSlim { set; get; } = new SemaphoreSlim(1);
    }
}