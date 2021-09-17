using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Prometheus
{
    public class MetricMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IMetricProxy _metricProxy;

        public MetricMiddleware(IMetricProxy metricProxy)
        {
            _metricProxy = metricProxy;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var response = await _metricProxy.ExecuteAsync(request.Name, next);
            return response;
        }
    }
}