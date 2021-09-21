using System;
using System.Collections.Concurrent;
using Finity.Shared.Metrics;
using Prometheus;
using Metrics = Prometheus.Metrics;

namespace Finity.Prometheus
{
    public class MetricHandler
    {
        private readonly ConcurrentDictionary<string, Counter> _counters = new();
        private readonly ConcurrentDictionary<string, Gauge> _gauges = new();

        public void SetMetric(MetricValue value)
        {
            BuildMetric((dynamic) value);
        }

        private void BuildMetric(GaugeAverageExecutionValue counterValue)
        {
            var name =
                $"{counterValue.MetricName}_" +
                $"{counterValue.ClientName}_" +
                $"{counterValue.RequestMessage.Method}"
                    .ToLower();

            if (!_gauges.TryGetValue(name, out var value))
            {
                value = Metrics.CreateGauge(name, string.Empty);
                _gauges.TryAdd(name, value);
            }

            
            var result = Math.Ceiling((value.Value * _counters.Count + counterValue.Value) / (_counters.Count + 1));
            value.Set(result);
        }
        
        private void BuildMetric(GaugeLastExecutionValue counterValue)
        {
            var name =
                $"{counterValue.MetricName}_" +
                $"{counterValue.ClientName}_" +
                $"{counterValue.RequestMessage.Method}"
                    .ToLower();

            if (!_gauges.TryGetValue(name, out var value))
            {
                value = Metrics.CreateGauge(name, string.Empty);
                _gauges.TryAdd(name, value);
            }

            
            value.SetToCurrentTimeUtc();
        }

        private void BuildMetric(CounterValue counterValue)
        {
            var name =
                $"{counterValue.MetricName}_" +
                $"{counterValue.ClientName}_" +
                $"{counterValue.RequestMessage.Method}"
                    .ToLower();

            if (!_counters.TryGetValue(name, out var value))
            {
                value = Metrics.CreateCounter(name, string.Empty);
                _counters.TryAdd(name, value);
            }

            value.Inc();
        }
    }
}