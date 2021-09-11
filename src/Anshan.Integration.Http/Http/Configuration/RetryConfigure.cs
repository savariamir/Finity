namespace Anshan.Integration.Http.Http.Configuration
{
    public class RetryConfigure 
    {
        public int RetryCount { get; set; }
        public int WaitingRetry { get; set; }
    }
    
    public class CircuitBreakerConfigure 
    {
    }
}