using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.Locking;
using Shemy.Pipeline;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Bulkhead
{
    public class BulkheadMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IOptionsSnapshot<AnshanFactoryOptions> _options;

        public BulkheadMiddleware(IOptionsSnapshot<AnshanFactoryOptions> options)
        {
            _options = options;
        }

        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            _options.Get(request.ClientName)
                .SemaphoreSlims.TryGetValue(request.ClientName, out var semaphore);
            
            lock (request.ClientName)
            {
                if (semaphore is {CurrentCount: 0})
                    return new HttpResponseMessage();
            }

            using (await semaphore.EnterAsync())
            {
                return await next();
            }
        }
    }
}