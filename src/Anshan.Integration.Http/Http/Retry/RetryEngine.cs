using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Clock;

namespace Anshan.Integration.Http.Http.Retry
{
    public class RetryEngine : IRetryEngine
    {
        private readonly IClock _clock;
        private readonly IRetryPolicy _retryPolicy;

        public RetryEngine(IClock clock, IRetryPolicy retryPolicy)
        {
            _clock = clock;
            _retryPolicy = retryPolicy;
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

                var waitDuration = new TimeSpan(100);
                if (waitDuration > TimeSpan.Zero)
                    await _clock.SleepAsync(new TimeSpan(100), cancellationToken);
            }

            return null;
        }
    }
}