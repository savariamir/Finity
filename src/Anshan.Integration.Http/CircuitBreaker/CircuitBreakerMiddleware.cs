using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Request;
using EasyPipe;

namespace Anshan.Integration.Http.CircuitBreaker
{
    internal class CircuitBreakerMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ICircuitBreakerEngine _engine;

        public CircuitBreakerMiddleware(ICircuitBreakerEngine engine)
        {
            _engine = engine;
        }

        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request,
                                                        IPipelineContext context,
                                                        Func<Task<HttpResponseMessage>> next,
                                                        CancellationToken cancellationToken)
        {
            return await _engine.ExecuteAsync(next);
        }
    }
}