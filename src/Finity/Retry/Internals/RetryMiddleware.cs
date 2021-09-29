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

        public RetryMiddleware(IClock clock, IOptionsSnapshot<RetryConfigure> options, ILogger<RetryMiddleware> logger)
        {
            _clock = clock;
            _options = options;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var firstResponse = await ExecuteFirstTryAsync(next);
            if (!firstResponse.IsSuccessful())
            {
                return await ExecuteNextTriesAsync(request, next,cancellationToken);
            }

            //Report Metrics for the first try
            // setMetric(new CounterValue());
            // Metrics.Increment(Metrics.FirstTryCount);
            return firstResponse;
        }

        private async Task<HttpResponseMessage> ExecuteFirstTryAsync(Func<Type, Task<HttpResponseMessage>> next)
        {
            var response = await next(MiddlewareType);
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
                var response = await next(MiddlewareType);
                if (response.IsSuccessful())
                {
                    //Report Metrics for next tries
                    _logger.LogInformation($"Succeeded after {i + 2} tries");
                    // setMetric(new CounterValue());
                    return response;
                }

                if (retryConfigure.SleepDurationRetry > TimeSpan.Zero)
                    await _clock.SleepAsync(retryConfigure.SleepDurationRetry, cancellationToken);
            }

            throw new RetryOutOfRangeException($"Fails after {retryConfigure.RetryCount} tries");
        }

        public Type MiddlewareType { get; set; }
            = typeof(RetryMiddleware);
    }
}