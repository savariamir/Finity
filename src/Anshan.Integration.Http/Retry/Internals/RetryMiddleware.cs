using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Retry.Appstractions;
using Anshan.Integration.Http.Retry.Configurations;
using Anshan.Integration.Http.Retry.Exceptions;
using EasyPipe;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Retry.Internals
{
    internal class RetryMiddleware : IMiddleware<AnshanHttpRequestMessage,HttpResponseMessage>
    {
        private readonly IClock _clock;
        private readonly IRetryPolicy _retryPolicy;
        private readonly RetryConfigure _retryConfigure;

        public RetryMiddleware(IClock clock, IRetryPolicy retryPolicy, IOptions<RetryConfigure> options)
        {
            _clock = clock;
            _retryPolicy = retryPolicy;
            _retryConfigure = options.Value;
        }

        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, 
                                                        IPipelineContext context, 
                                                        Func<Task<HttpResponseMessage>> next, 
                                                        CancellationToken cancellationToken)
        {
            while (_retryPolicy.CanRetry())
            {
                var response = await next();

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