using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.CircuitBreaker.Abstractions;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;
using Finity.Shared.Metrics;

namespace Finity.CircuitBreaker.Internals
{
    public class CircuitBreakerMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ICircuitBreakerEngine _engine;

        public CircuitBreakerMiddleware(ICircuitBreakerEngine engine)
        {
            _engine = engine;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            return await _engine.ExecuteAsync(request, next);
        }

        public Type MiddlewareType { get; set; } =
            typeof(CircuitBreakerMiddleware);
    }
}