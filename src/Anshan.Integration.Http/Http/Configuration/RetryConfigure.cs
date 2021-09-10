namespace Anshan.Integration.Http.Configuration
{
    public class RetryConfigure 
    {
        public int RetryCount { get; set; }
        public bool WaitingRetry { get; set; }
    }
    
    public class CircuitBreakerConfigure 
    {
    }
}