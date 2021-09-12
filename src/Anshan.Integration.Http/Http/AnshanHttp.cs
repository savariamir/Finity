using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Anshan.Integration.Http.Http.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Anshan.Integration.Http.Http
{
    public class AnshanHttp : IAnshanHttp
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public AnshanHttp(IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            _cache = cache;
            _httpClient = clientFactory.CreateClient("test");
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var cacheValue =
                GetFromCache<T>(CacheKey.GetKey(uri));
            if (cacheValue.Hit)
                return cacheValue.Data;
            
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
            if (!response.IsSuccessStatusCode) throw new Exception();
            
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<T>(responseStream);
                
            SetCache(data,CacheKey.GetKey(uri));

            return data;
        }
        
        private CacheModel<T> GetFromCache<T>(string url)
        {
           var cacheKey= CacheKey.GetKey(url);
            var data = _cache.Get<T>(cacheKey);
            return new CacheModel<T>(data);
        }

        private void SetCache<T>(T response, string url)
        {
            var cacheKey= CacheKey.GetKey(url);
            _cache.Set(cacheKey, response);
        }
    }
}