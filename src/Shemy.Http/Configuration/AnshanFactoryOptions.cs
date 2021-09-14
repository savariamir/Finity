namespace Shemy.Http.Configuration
{
    public class AnshanFactoryOptions
    {
        public bool IsCacheEnabled { set; get; }
        public bool IsRetryEnabled { set; get; }
        public bool IsCircuitBreakerEnabled { get; set; }
    }
}