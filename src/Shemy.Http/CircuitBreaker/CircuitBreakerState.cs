namespace Shemy.Http.CircuitBreaker
{
    internal enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}