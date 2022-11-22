using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Finity.Authentication.Abstractions;
using Finity.Authentication.Configurations;
using Finity.Caching.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using CacheKey = Finity.Authentication.Configurations.CacheKey;

namespace Finity.Authentication.Internals
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
            _httpClient = clientFactory.CreateClient("finity-auth");
        }

        public async Task<string> GetTokenAsync(string name, CancellationToken cancellationToken = default)
        {
            var cacheResult = GetFromCache(name);
            if (cacheResult.Hit)
                return cacheResult.Data;

            var token = await RequestTokenAsync(name, cancellationToken);
            SetToCache(token, name);

            return token;
        }

        public async Task<string> GetNewTokenAsync(string name, CancellationToken cancellationToken)
        {
            var token = await RequestTokenAsync(name, cancellationToken);
            SetToCache(token, name);

            return token;
        }

        private async Task<string> RequestTokenAsync(string name, CancellationToken cancellationToken = default)
        {
            var configure = _options.Get(name);
            var data = new[]
            {
                new KeyValuePair<string, string>("client_id", configure.ClientId),
                new KeyValuePair<string, string>("client_secret", configure.ClientSecret),
                new KeyValuePair<string, string>("scope", configure.Scope),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            };
            var result =
                await _httpClient.PostAsync(configure.Endpoint, new FormUrlEncodedContent(data), cancellationToken);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync(cancellationToken);
            var response = JsonSerializer.Deserialize<TokenResponse>(content);

            if (response is null)
            {
                throw new Exception();
            }

            return response.AccessToken;
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