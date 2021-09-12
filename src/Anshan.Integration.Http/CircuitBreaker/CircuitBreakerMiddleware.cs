using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyPipe;

namespace Anshan.Integration.Http.CircuitBreaker
{
    public class CircuitBreakerMiddleware: IMiddleware<AnshanHttpRequestMessage,HttpResponseMessage>
    {
        public Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context, Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}