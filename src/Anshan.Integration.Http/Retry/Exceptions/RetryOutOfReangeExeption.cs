using System;

namespace Anshan.Integration.Http.Retry.Exceptions
{
    internal class RetryOutOfRangeException : Exception
    {
        public RetryOutOfRangeException(string message = "Retry Out of Range Exception") : base(message)
        {
            
        }
    }
}