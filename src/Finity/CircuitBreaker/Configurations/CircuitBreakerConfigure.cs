using System;

namespace Finity.CircuitBreaker.Configurations
{
    public class CircuitBreakerConfigure
    {
        public int ExceptionsAllowedBeforeBreaking { set; get; }
        public int SuccessAllowedBeforeClosing { set; get; }
        public TimeSpan DurationOfBreak { set; get; }
    }
}