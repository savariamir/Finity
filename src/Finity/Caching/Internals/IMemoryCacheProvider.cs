using System.Net.Http;
using Finity.Caching.Abstractions;
using Finity.Caching.Configurations;
using Finity.Request;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Finity.Caching.Internals
{
    internal interface IMemoryCacheProvider
    {
        void SetToCache(FinityHttpRequestMessage request, HttpResponseMessage response);
        CacheResult<HttpResponseMessage> GetFromCache(string requestUri);
    }

    internal class MemoryCacheProvider : IMemoryCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly IOptionsSnapshot<CacheConfigure> _options;

        public MemoryCacheProvider(IMemoryCache cache, IOptionsSnapshot<CacheConfigure> options)
        {
            _cache = cache;
            _options = options;
        }

        public void SetToCache(FinityHttpRequestMessage request, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return;

            var cacheConfigure = _options.Get(request.Name);

            if (request.HttpRequest.RequestUri is not null)
                _cache.Set(CacheKey.GetKey(request.HttpRequest.RequestUri.ToString()), response,
                    cacheConfigure.AbsoluteExpirationRelativeToNow);
        }

        public CacheResult<HttpResponseMessage> GetFromCache(string requestUri)
        {
            var cacheKey = CacheKey.GetKey(requestUri);
            var data = _cache.Get<HttpResponseMessage>(cacheKey);
            return new CacheResult<HttpResponseMessage>(data);
        }
    }
}