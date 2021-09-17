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
        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var response = await next();
            return response;
        }
    }
}