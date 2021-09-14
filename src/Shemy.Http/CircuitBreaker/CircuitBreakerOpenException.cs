using System;

namespace Shemy.Http.CircuitBreaker
{
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }
    
    public class CircuitBreakerException : Exception
    {
        public CircuitBreakerException(string message) : base(message)
        {
        }
        
        public CircuitBreakerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}