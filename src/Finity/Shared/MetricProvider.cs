using System;
using Finity.Shared.Metrics;

namespace Finity.Shared
{
    public interface IMetricProvider
    {
        void AddMetric(MetricValue value);
    }

    public class MetricProvider: IMetricProvider
    {
        private readonly Action<MetricValue> _action;

        public MetricProvider(Action<MetricValue> action)
        {
            _action = action;
        }

        public void AddMetric(MetricValue value)
        {
            _action(value);
        }
    }
}