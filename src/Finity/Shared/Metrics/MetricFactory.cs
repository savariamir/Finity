using System.Net.Http;

namespace Finity.Shared.Metrics
{
    public class MetricFactory
    {
        public static GaugeValue CreateGauge()
        {
            return new GaugeValue();
        }

        public static GaugeLastExecutionValue CreateGaugeLastExecutionValue(
            string name,
            HttpRequestMessage requestMessage, string metricName, string help)
        {
            return new()
            {
                ClientName = name,
                RequestMessage = requestMessage,
                MetricName = metricName,
                Help = help
            };
        }
        
        public static CounterValue CreateCounter(string name,
            HttpRequestMessage requestMessage, string metricName, string help)
        {
            return new()
            {
                ClientName = name,
                RequestMessage = requestMessage,
                MetricName = metricName,
                Help = help
            };
        }

        public static GaugeAverageExecutionValue CreateGaugeAverageExecutionValue(string name,
            HttpRequestMessage requestMessage, string metricName, string help, double value)
        {
            return new()
            {
                ClientName = name,
                RequestMessage = requestMessage,
                MetricName = metricName,
                Value = value,
                Help = help
            };
        }
    }
}