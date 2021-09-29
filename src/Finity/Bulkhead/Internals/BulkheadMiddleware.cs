using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Bulkhead.Abstractions;
using Finity.Locking;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared.Metrics;

namespace Finity.Bulkhead.Internals
{
    public class BulkheadMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IBulkheadLockProvider _bulkheadLockProvider;

        public BulkheadMiddleware(IBulkheadLockProvider bulkheadLockProvider)
        {
            _bulkheadLockProvider = bulkheadLockProvider;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            using (await _bulkheadLockProvider
                .TrySemaphore(request.Name)
                .EnterAsync(cancellationToken))
            {
                return await next(Type);
            }
        }

        public Type Type { get; set; }
            = typeof(BulkheadMiddleware);
    }
}