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
        private readonly Action<MetricValue> _action;

        public MetricProxy(Action<MetricValue> action)
        {
            _action = action;
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

            _action(MetricFactory.CreateGaugeAverageExecutionValue(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.AverageExecutionTimeMilliseconds,
                string.Empty,
                _stopwatch.Elapsed.Milliseconds));

            _action(MetricFactory.CreateCounter(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.TotalNumberOfExecutions,
                string.Empty));


            _action(MetricFactory.CreateGaugeLastExecutionValue(
                request.Name,
                request.HttpRequest,
                Metrics.Metrics.LastExecution,
                string.Empty));
        }
    }
}