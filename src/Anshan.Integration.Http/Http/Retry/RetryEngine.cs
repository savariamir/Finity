using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Http.Configuration;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Http.Retry
{
    public class RetryEngine : IRetryEngine
    {
        private readonly IClock _clock;
        private readonly IRetryPolicy _retryPolicy;
        private readonly int _waitingRetry;

        public RetryEngine(IClock clock, IRetryPolicy retryPolicy,IOptions<RetryConfigure> options)
        {
            _clock = clock;
            _retryPolicy = retryPolicy;
            _waitingRetry = options.Value.WaitingRetry;
        }

        public async Task<HttpResponseMessage> Retry(
            Func<Task<HttpResponseMessage>> sendAsync,
            CancellationToken cancellationToken)
        {
            while (_retryPolicy.CanRetry())
            {
                var response = await sendAsync();

                if (response.IsSuccessStatusCode)
                    return response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Don't reattempt a bad request
                    return response;
                }

                var waitDuration = new TimeSpan(_waitingRetry);
                if (waitDuration > TimeSpan.Zero)
                    await _clock.SleepAsync(waitDuration, cancellationToken);
            }

            throw new Exception("Retry is zero");
        }
    }
}