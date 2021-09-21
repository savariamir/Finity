using System.Net.Http;

namespace Finity.Shared.Metrics
{
    public abstract class MetricValue
    {
        public string ClientName { get; set; }
        public string MetricName { get; set; }
        
        public string  Help { get; set; } 
        public HttpRequestMessage RequestMessage { set; get; }
    }
}