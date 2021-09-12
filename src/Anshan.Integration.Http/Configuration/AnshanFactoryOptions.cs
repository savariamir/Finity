namespace Anshan.Integration.Http.Configuration
{
    public class AnshanFactoryOptions
    {
        public bool IsCacheEnabled { set; get; }
        public bool IsRetryEnabled { set; get; }
        public bool CircuitBreaker { get; set; }
    }
}