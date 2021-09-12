using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http
{
    internal class HttpDelegationHandler : DelegatingHandler
    {
        private readonly IHttpEngine _httpEngine;

        public HttpDelegationHandler(IHttpEngine httpEngine)
        {
            _httpEngine = httpEngine;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await _httpEngine.SendAsync(request, cancellationToken,
                () => base.SendAsync(request, cancellationToken));

            return response;
        }
    }
}