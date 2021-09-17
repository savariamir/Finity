namespace Shemy.CircuitBreaker.Internals
{
    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}