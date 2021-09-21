namespace Finity.Bulkhead.Configurations
{
    public class BulkheadConfigure
    {
        public int MaxConcurrentCalls { set; get; }
        public int MaxWaitDuration { set; get; }
    }
}