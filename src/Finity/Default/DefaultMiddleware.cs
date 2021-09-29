using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;
using Finity.Shared.Metrics;

namespace Finity.Default
{
    public class DefaultMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IMetricProxy _metricProxy;

        public DefaultMiddleware(IMetricProxy metricProxy)
        {
            _metricProxy = metricProxy;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var response = await _metricProxy.ExecuteAsync(request);
            return response;
        }

        public Type MiddlewareType { get; set; }
            = typeof(DefaultMiddleware);
    }
}