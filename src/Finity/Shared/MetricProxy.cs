using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finity.Shared
{
    public class MetricProxy: IMetricProxy
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
            _action(new CounterValue());
            _action(new GaugeValue());
            // _reporter.Report(name, _stopwatch.Elapsed.Milliseconds);
        }
    }
}