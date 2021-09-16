using System;

namespace Shemy.CircuitBreaker
{
    internal class MetricItem
    {
        public int Successes { get; set; }

        public int Failures { get; set; }

        public CircuitBreakerState CircuitBreakerState { get; set; } = CircuitBreakerState.Closed;

        public DateTime LastFailureDateTimeUtc { get; set; }
    }
}