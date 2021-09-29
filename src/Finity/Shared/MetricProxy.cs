using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Finity.Request;
using Finity.Shared.Metrics;

namespace Finity.Shared
{
    public class MetricProxy : IMetricProxy
    {
        private Stopwatch _stopwatch;
        private readonly IMetricProvider _metricProvider;

        public MetricProxy(IMetricProvider metricProvider)
        {
            _metricProvider = metricProvider;
        }

        protected virtual void Before()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public async Task<HttpResponseMessage> ExecuteAsync(FinityHttpRequestMessage request)
        {
            Before();
            var response = await request.SendAsync();
            After(request);

            return response;
        }

        protected virtual void After(FinityHttpRequestMessage request)
        {
            _stopwatch.Stop();

            _metricProvider.AddMetric(MetricFactory.CreateGaugeAverageExecutionValue(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.AverageExecutionTimeMilliseconds,
                string.Empty,
                _stopwatch.Elapsed.Milliseconds));

            _metricProvider.AddMetric(MetricFactory.CreateCounter(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.TotalNumberOfExecutions,
                string.Empty));


            _metricProvider.AddMetric(MetricFactory.CreateGaugeLastExecutionValue(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.LastExecution,
                string.Empty));
        }
    }
}