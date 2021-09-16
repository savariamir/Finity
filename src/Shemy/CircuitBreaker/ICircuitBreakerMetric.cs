using System;

namespace Shemy.CircuitBreaker
{
    internal interface ICircuitBreakerMetric
    {
        void SetState(CircuitBreakerState state,string name);
        void IncrementSuccess(string name);
        void IncrementFailure(string name);
        int GetFailures(string name);
        int GetSuccess(string name);
        DateTime GetLastFailureDateTimeUtc(string name);
        void Reset(string name);
        CircuitBreakerState GetState(string name);
    }
}