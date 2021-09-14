using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Request;
using Anshan.Integration.Pipeline.Abstractions;

namespace Anshan.Integration.Http.Metrics
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