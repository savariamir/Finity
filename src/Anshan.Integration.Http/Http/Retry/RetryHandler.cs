using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.Http.Retry
{
    public class RetryHandler : DelegatingHandler
    {
        private readonly IRetryEngine _retryEngine;

        public RetryHandler(IRetryEngine retryEngine) 
        {
            _retryEngine = retryEngine;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await _retryEngine.Retry(() =>
                    base.SendAsync(request, cancellationToken), cancellationToken);

                return response;
            }
            catch(Exception ex)
            {
                // ignored
            }


            return response;
        }
    }
}