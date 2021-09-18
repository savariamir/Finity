using System;

namespace Finity.CircuitBreaker.Abstractions
{
    public interface ICircuitBreakerMetric
    {
        void IncrementSuccess(string name);
        void IncrementFailure(string name);
        int GetFailures(string name);
        int GetSuccess(string name);
        DateTime GetLastFailureDateTimeUtc(string name);
    }
}