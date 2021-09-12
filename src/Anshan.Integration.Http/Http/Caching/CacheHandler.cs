using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Anshan.Integration.Http.Http.Caching
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

            if (request.Method == HttpMethod.Get)
            {
                var cacheValue =
                    GetFromCache(CacheKey.GetKey(request.RequestUri.ToString()));
                if (cacheValue.Hit)
                    return cacheValue.Data;
            }


            var response = await base.SendAsync(request, cancellationToken);
            SetCache(response, CacheKey.GetKey(request.RequestUri.ToString()));

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