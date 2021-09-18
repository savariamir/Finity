using System;

namespace Finity.CircuitBreaker.Exceptions
{
    internal class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }
}