namespace Anshan.Integration.Http.CircuitBreaker
{
    internal enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}