using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Clock;
using Finity.Extensions;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Retry.Configurations;
using Finity.Retry.Exceptions;
using Finity.Shared;
using Finity.Shared.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Finity.Retry.Internals
{
    public class RetryMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IClock _clock;
        private readonly IOptionsSnapshot<RetryConfigure> _options;
        private readonly ILogger<RetryMiddleware> _logger;
        private readonly IMetricProvider _metricProvider;

        public RetryMiddleware(IClock clock, IOptionsSnapshot<RetryConfigure> options, ILogger<RetryMiddleware> logger,
            IMetricProvider metricProvider)
        {
            _clock = clock;
            _options = options;
            _logger = logger;
            _metricProvider = metricProvider;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var firstTry = await ExecuteFirstTryAsync(next);
            if (!firstTry.IsSuccessful())
            {
                return await ExecuteNextTriesAsync(request, next, cancellationToken);
            }
            
            _logger.LogInformation($"Succeeded after first try");
            _metricProvider.AddMetric(MetricFactory.CreateCounter(request.Name, request.HttpRequest,
                Metrics.FirstTryCount, string.Empty));
            
            return firstTry;
        }

        private async Task<HttpResponseMessage> ExecuteFirstTryAsync(Func<Type, Task<HttpResponseMessage>> next)
        {
            var response = await next(Type);
            return response;
        }

        private async Task<HttpResponseMessage> ExecuteNextTriesAsync(
            FinityHttpRequestMessage request,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var retryConfigure = _options.Get(request.Name);
            for (var i = 0; i < retryConfigure.RetryCount; i++)
            {
                var response = await next(Type);
                if (response.IsSuccessful())
                {
                    _logger.LogInformation($"Succeeded after {i + 2} tries");
                    _metricProvider.AddMetric(MetricFactory.CreateCounter(request.Name, request.HttpRequest,
                        Metrics.NextTryCount, string.Empty));
                    return response;
                }

                if (retryConfigure.SleepDurationRetry > TimeSpan.Zero)
                    await _clock.SleepAsync(retryConfigure.SleepDurationRetry, cancellationToken);
            }

            throw new RetryOutOfRangeException($"Fails after {retryConfigure.RetryCount} tries");
        }

        public Type Type { get; set; }
            = typeof(RetryMiddleware);
    }
}