using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Finity.Caching
{
    public class MemoryCacheMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IMemoryCache _cache;
        private readonly IOptionsSnapshot<CacheConfigure> _options;
        private readonly ILogger<MemoryCacheMiddleware> _logger;

        public MemoryCacheMiddleware(IMemoryCache cache, IOptionsSnapshot<CacheConfigure> options, ILogger<MemoryCacheMiddleware> logger)
        {
            _cache = cache;
            _options = options;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Type,Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            if (request.HttpRequest.RequestUri is null) throw new Exception("");

            var cacheValue =
                GetFromCache(CacheKey.GetKey(request.HttpRequest.RequestUri.ToString()));

            if (cacheValue.Hit)
            {
                //Report that cache hits
                _logger.LogInformation("Data is read from cache", DateTimeOffset.UtcNow);
                return cacheValue.Data;
            }

            var response = await next(MiddlewareType);
            SetToCache(request, response);

            return response;
        }

        private void SetToCache(AnshanHttpRequestMessage request, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return;

            var cacheConfigure = _options.Get(request.Name);

            if (request.HttpRequest.RequestUri is not null)
                _cache.Set(CacheKey.GetKey(request.HttpRequest.RequestUri.ToString()), response,
                    cacheConfigure.AbsoluteExpirationRelativeToNow);
        }

        private CacheResult<HttpResponseMessage> GetFromCache(string cacheKey)
        {
            var data = _cache.Get<HttpResponseMessage>(cacheKey);
            return new CacheResult<HttpResponseMessage>(data);
        }
        
       public Type MiddlewareType { get; set; } 
            = typeof(MemoryCacheMiddleware);
    }
}