using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Core;
using Finity.Locking;
using Finity.Pipeline.Abstractions;
using Finity.Request;

namespace Finity.Bulkhead
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
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            using (await _bulkheadLockProvider
                .TrySemaphore(request.Name)
                .EnterAsync(cancellationToken))
            {
                return await next();
            }
        }
    }
}