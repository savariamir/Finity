using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Caching
{
    internal class MemoryCacheMiddleware: IMiddleware<AnshanHttpRequestMessage,HttpResponseMessage>
    {
        private readonly IMemoryCache _cache;
        private readonly IOptionsSnapshot<CacheConfigure> _options;
        
        public MemoryCacheMiddleware(IMemoryCache cache, IOptionsSnapshot<CacheConfigure> options)
        {
            _cache = cache;
            _options = options;
        }
        
        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, 
                                                        IPipelineContext context, 
                                                        Func<Task<HttpResponseMessage>> next, 
                                                        CancellationToken cancellationToken)
        {
            if (request.HttpRequestMessage.RequestUri is null) throw new Exception("");
            if (request.HttpRequestMessage.Method != HttpMethod.Get) return null;

            var cacheValue =
                GetFromCache(CacheKey.GetKey(request.HttpRequestMessage.RequestUri.ToString()));

            if (cacheValue.Hit)
                return cacheValue.Data;

            var response = await next();
            
            SetToCache(request, response);

            return response;
        }
        
        private void SetToCache(AnshanHttpRequestMessage request,HttpResponseMessage response)
        {
            if (response is null || !response.IsSuccessStatusCode) return;

            var cacheConfigure = _options.Get(request.ClientName);
            
            if (request.HttpRequestMessage.RequestUri is not null)
                _cache.Set(CacheKey.GetKey(request.HttpRequestMessage.RequestUri.ToString()), response, cacheConfigure.AbsoluteExpirationRelativeToNow);
        }

        private CacheResult<HttpResponseMessage> GetFromCache(string cacheKey)
        {
            var data = _cache.Get<HttpResponseMessage>(cacheKey);
            return new CacheResult<HttpResponseMessage>(data);
        }
    }
}