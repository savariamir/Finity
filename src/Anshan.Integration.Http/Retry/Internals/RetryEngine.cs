using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Retry.Appstractions;
using Anshan.Integration.Http.Retry.Configurations;
using Anshan.Integration.Http.Retry.Exceptions;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Retry.Internals
{
    internal class RetryEngine : IRetryEngine
    {
        private readonly IClock _clock;
        private readonly IRetryPolicy _retryPolicy;
        private readonly RetryConfigure _retryConfigure;

        public RetryEngine(IClock clock, IRetryPolicy retryPolicy, IOptions<RetryConfigure> options)
        {
            _clock = clock;
            _retryPolicy = retryPolicy;
            _retryConfigure = options.Value;
        }

        public async Task<HttpResponseMessage> Retry(
            Func<Task<HttpResponseMessage>> sendAsync,
            CancellationToken cancellationToken)
        {
            while (_retryPolicy.CanRetry() && cancellationToken.CanBeCanceled)
            {
                var response = await sendAsync();

                if (response.IsSuccessStatusCode)
                    return response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Don't reattempt a bad request
                    return response;
                }

                if (_retryConfigure.SleepDurationRetry > TimeSpan.Zero)
                    await _clock.SleepAsync(_retryConfigure.SleepDurationRetry, cancellationToken);
            }

            throw new RetryOutOfRangeException();
        }
    }
}