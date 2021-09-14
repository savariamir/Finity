using System;

namespace Shemy.Http.Retry.Exceptions
{
    internal class RetryOutOfRangeException : Exception
    {
        public RetryOutOfRangeException(string message = "Retry Out of Range Exception") : base(message)
        {
            
        }
    }
}