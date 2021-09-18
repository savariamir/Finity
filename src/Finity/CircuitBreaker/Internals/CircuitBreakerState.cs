namespace Finity.CircuitBreaker.Internals
{
    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}