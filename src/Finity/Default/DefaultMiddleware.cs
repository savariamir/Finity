using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;

namespace Finity.Default
{
    public class DefaultMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IMetricProxy _metricProxy;

        public DefaultMiddleware(IMetricProxy metricProxy)
        {
            _metricProxy = metricProxy;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            var response = await _metricProxy.ExecuteAsync(request.Name, request.SendAsync);
            return response;
        }
    }
}