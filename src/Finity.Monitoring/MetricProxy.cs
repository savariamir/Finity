using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Finity.Monitoring;

namespace Finity.Prometheus
{
    public class MetricProxy : IMetricProxy
    {
        private Stopwatch _stopwatch;
        private readonly MetricReporter _reporter;

        public MetricProxy(MetricReporter reporter)
        {
            _reporter = reporter;
        }

        protected virtual void Before()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            string name,
            Func<Task<HttpResponseMessage>> func)
        {
            Before();
            var response = await func();
            After(name);

            return response;
        }

        protected virtual void After(string name)
        {
            _stopwatch.Stop();
            _reporter.Report(name, _stopwatch.Elapsed.Milliseconds);
        }
    }
}