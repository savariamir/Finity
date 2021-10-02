using System;

namespace Finity.Retry.Exceptions
{
    public class RetryOutOfRangeException : Exception
    {
        public RetryOutOfRangeException(string message = "Retry Out of Range Exception") : base(message)
        {
            
        }
    }
}