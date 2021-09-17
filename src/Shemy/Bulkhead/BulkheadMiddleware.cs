using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Locking;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Bulkhead
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
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            using (await _bulkheadLockProvider
                .TrySemaphore(request.ClientName)
                .EnterAsync(cancellationToken))
            {
                return await next();
            }
        }
    }
}