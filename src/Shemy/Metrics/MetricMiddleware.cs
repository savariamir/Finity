using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Metrics
{
    public class MetricMiddleware: IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        public Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context, Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}