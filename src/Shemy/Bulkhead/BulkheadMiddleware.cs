using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Bulkhead
{
    public class BulkheadMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var semaphore = new SemaphoreSlim(0, 10);

            using (await semaphore.EnterAsync())
                return await next();
        }
    }
}