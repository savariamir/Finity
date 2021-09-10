using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Anshan.Integration.Http.Caching
{
    public class CacheHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;

        public CacheHandler(IMemoryCache cache)
        {
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.RequestUri is null) throw new Exception("");


            var cacheKey = CacheKey.GetKey(request.RequestUri.ToString());
            var cacheValue = GetFromCache(cacheKey);
            if (cacheValue.Hit)
                return cacheValue.Data;

            var response = await base.SendAsync(request, cancellationToken);
            SetCache(response, cacheKey);

            return response;
        }

        private CacheModel<HttpResponseMessage> GetFromCache(string cacheKey)
        {
            var data = _cache.Get<HttpResponseMessage>(cacheKey);
            return new CacheModel<HttpResponseMessage>(data);
        }

        private void SetCache(HttpResponseMessage response, string cacheKey)
        {
            if (response is not null && response.IsSuccessStatusCode)
            {
                _cache.Set(cacheKey, response);
            }
        }
    }
}