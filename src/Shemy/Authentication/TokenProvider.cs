using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shemy.Caching;

namespace Shemy.Authentication
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly IMemoryCache _cache;
        private readonly IOptionsSnapshot<AuthenticationConfigure> _options;
        private readonly HttpClient _httpClient;

        public TokenProvider(IMemoryCache cache, IOptionsSnapshot<AuthenticationConfigure> options,
            IHttpClientFactory clientFactory)
        {
            _cache = cache;
            _options = options;
            _httpClient = clientFactory.CreateClient("shemy-auth");
        }

        public Task<string> GetToken(string clientName)
        {
            throw new System.NotImplementedException();
        }

        internal async Task<HttpResponseMessage> RequestTokenAsync(HttpRequestMessage request,
            CancellationToken cancellationToken = default)
        {
            request.Method = HttpMethod.Post;

            var response = await _httpClient.SendAsync(request, cancellationToken);

            return response;
        }

        private void SetToCache(string token, string clientName)
        {
            var configure = _options.Get(clientName);
            _cache.Set(CacheKey.GetKey(clientName), token, configure.AbsoluteExpirationRelativeToNow);
        }

        private CacheResult<string> GetFromCache(string clientName)
        {
            var data = _cache.Get<string>(CacheKey.GetKey(clientName));
            return new CacheResult<string>(data);
        }
    }
}