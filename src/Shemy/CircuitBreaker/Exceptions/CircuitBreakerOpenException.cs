using System;

namespace Shemy.CircuitBreaker.Exceptions
{
    internal class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }
}