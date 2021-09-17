namespace Shemy.Bulkhead
{
    public class BulkheadConfigure
    {
        public int MaxConcurrentCalls { set; get; }
        public int MaxWaitDuration { set; get; }
    }
}