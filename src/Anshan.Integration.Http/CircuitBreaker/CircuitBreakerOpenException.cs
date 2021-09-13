using System;

namespace Anshan.Integration.Http.CircuitBreaker
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