using System;

namespace Anshan.Integration.Pipeline.Exceptions
{
    [Serializable]
    public class MiddlewareNotResolvedException : Exception
    {
        public MiddlewareNotResolvedException(Type middlewareType) :
            base($"middleware of type '{middlewareType.Name}' could not be resolved")
        {
            
        }
    }
}