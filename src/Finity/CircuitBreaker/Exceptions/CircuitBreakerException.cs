using System;

namespace Finity.CircuitBreaker.Exceptions
{
    internal class CircuitBreakerException : Exception
    {
        public CircuitBreakerException(string message) : base(message)
        {
        }
        
        public CircuitBreakerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}