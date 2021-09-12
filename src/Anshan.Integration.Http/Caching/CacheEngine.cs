using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Caching
{
    internal class CacheEngine : ICacheEngine
    {
        private readonly IMemoryCache _cache;
        private readonly CacheConfigure _cacheConfigure; 

        public CacheEngine(IMemoryCache cache, IOptions<CacheConfigure> options)
        {
            _cache = cache;
            _cacheConfigure = options.Value;
        }

        public CacheResult<HttpResponseMessage> GetFromCache(HttpRequestMessage request)
        {
            if (request.RequestUri is null) throw new Exception("");

            if (request.Method != HttpMethod.Get) return new CacheResult<HttpResponseMessage>(null);
            
            
            var cacheValue =
                GetFromCache(CacheKey.GetKey(request.RequestUri.ToString()));
            
            return cacheValue.Hit ? cacheValue : new CacheResult<HttpResponseMessage>(null);
        }

        public void Set(HttpRequestMessage request,HttpResponseMessage response)
        {
            if (response is null || !response.IsSuccessStatusCode) return;
            
            if (request.RequestUri is not null)
                _cache.Set(CacheKey.GetKey(request.RequestUri.ToString()), response, _cacheConfigure.AbsoluteExpirationRelativeToNow);
        }

        private CacheResult<HttpResponseMessage> GetFromCache(string cacheKey)
        {
            var data = _cache.Get<HttpResponseMessage>(cacheKey);
            return new CacheResult<HttpResponseMessage>(data);
        }
        
    }
}