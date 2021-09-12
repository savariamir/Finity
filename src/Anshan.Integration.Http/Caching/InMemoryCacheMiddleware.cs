using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyPipe;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Caching
{
    public class InMemoryCacheMiddleware: IMiddleware<AnshanHttpRequestMessage,HttpResponseMessage>
    {
        private readonly IMemoryCache _cache;
        private readonly CacheConfigure _cacheConfigure; 
        
        public InMemoryCacheMiddleware(IMemoryCache cache, IOptions<CacheConfigure> options)
        {
            _cache = cache;
            _cacheConfigure = options.Value;
        }
        
        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, 
                                                        IPipelineContext context, 
                                                        Func<Task<HttpResponseMessage>> next, 
                                                        CancellationToken cancellationToken)
        {
            if (request.Request.RequestUri is null) throw new Exception("");

            if (request.Request.Method != HttpMethod.Get) return null;
            
            
            var cacheValue =
                GetFromCache(CacheKey.GetKey(request.Request.RequestUri.ToString()));

            if (cacheValue.Hit)
                return null;

            var response = await next();
            
            Set(request.Request, response);

            return response;
        }
        
        private void Set(HttpRequestMessage request,HttpResponseMessage response)
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