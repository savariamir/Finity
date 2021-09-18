using System;

namespace Finity.CircuitBreaker.Internals
{
    internal class MetricItem
    {
        public int Successes { get; set; }

        public int Failures { get; set; }

        public DateTime LastFailureDateTimeUtc { get; set; }
    }
}