using System;

namespace Shemy.CircuitBreaker
{
    internal class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }
    
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