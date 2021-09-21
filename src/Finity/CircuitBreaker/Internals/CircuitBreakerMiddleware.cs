using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.CircuitBreaker.Abstractions;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;

namespace Finity.CircuitBreaker.Internals
{
    public class CircuitBreakerMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ICircuitBreakerEngine _engine;

        public CircuitBreakerMiddleware(ICircuitBreakerEngine engine)
        {
            _engine = engine;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            return await _engine.ExecuteAsync(request, next);
        }

        public Type MiddlewareType { get; set; } =
            typeof(CircuitBreakerMiddleware);
    }
}