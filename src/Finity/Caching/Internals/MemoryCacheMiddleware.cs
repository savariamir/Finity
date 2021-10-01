using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Caching.Abstractions;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;
using Finity.Shared.Metrics;
using Microsoft.Extensions.Logging;

namespace Finity.Caching.Internals
{
    public class MemoryCacheMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IMemoryCacheProvider _cache;
        private readonly ILogger<MemoryCacheMiddleware> _logger;
        private readonly IMetricProvider _metricProvider;

        public MemoryCacheMiddleware(
            IMemoryCacheProvider cache,
            ILogger<MemoryCacheMiddleware> logger,
            IMetricProvider metricProvider)
        {
            _cache = cache;
            _logger = logger;
            _metricProvider = metricProvider;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            if (request.HttpRequest.RequestUri is null) throw new Exception("Request uri is not allowed to be empty");

            // Get Http Method is just going to be cached
            if (request.HttpRequest.Method != HttpMethod.Get)
            {
                return await next(Type);
            }

            var cacheValue =
                _cache.GetFromCache(request.HttpRequest.RequestUri.ToString());

            if (cacheValue.Miss)
            {
                var response = await next(Type);
                _cache.SetToCache(request, response);
                return response;
            }

            _metricProvider
                .AddMetric(
                    MetricFactory
                        .CreateCounter(request.Name, request.HttpRequest, Metrics.CacheHit, string.Empty));

            _logger.LogInformation("Data was read from cache", DateTimeOffset.UtcNow);
            return cacheValue.Data;
        }

        public Type Type { get; set; } = typeof(MemoryCacheMiddleware);

    }
}