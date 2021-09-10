using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.Retry
{
    public class RetryHandler : DelegatingHandler
    {
        private readonly IRetryEngine _retryEngine;

        public RetryHandler(HttpMessageHandler innerHandler,
            IRetryEngine retryEngine) : base(innerHandler)
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
            catch
            {
                // ignored
            }


            return response;
        }
    }
}