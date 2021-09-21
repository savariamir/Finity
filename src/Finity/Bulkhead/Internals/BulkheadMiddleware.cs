using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Bulkhead.Abstractions;
using Finity.Locking;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;

namespace Finity.Bulkhead.Internal
{
    public class BulkheadMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IBulkheadLockProvider _bulkheadLockProvider;

        public BulkheadMiddleware(IBulkheadLockProvider bulkheadLockProvider)
        {
            _bulkheadLockProvider = bulkheadLockProvider;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            using (await _bulkheadLockProvider
                .TrySemaphore(request.Name)
                .EnterAsync(cancellationToken))
            {
                return await next(MiddlewareType);
            }
        }

        public Type MiddlewareType { get; set; }
            = typeof(BulkheadMiddleware);
    }
}