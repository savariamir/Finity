namespace Shemy.CircuitBreaker
{
    internal enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}