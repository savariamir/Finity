using System.Net.Http;
using System.Text.Json;
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

        public async Task<string> GetToken(string name,CancellationToken cancellationToken = default)
        {
            var cacheResult = GetFromCache(name);
            if (cacheResult.Hit)
                return cacheResult.Data;
            
            var token = await RequestTokenAsync(cancellationToken);
            
            SetToCache(token, name);

            return token;
        }

        private async Task<string> RequestTokenAsync(CancellationToken cancellationToken = default)
        {
            var content = new StringContent("application/json");

            using var response =
                await _httpClient.PostAsync("/api/TodoItems", content, cancellationToken);
            
            var responseStream =await  response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<TokenResponse>(responseStream);
            
            return data.AccessToken;
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