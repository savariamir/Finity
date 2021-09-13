using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Request;
using Anshan.Integration.Http.Retry.Configurations;
using Anshan.Integration.Http.Retry.Exceptions;
using EasyPipe;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Retry.Internals
{
    internal class RetryMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly IClock _clock;
        private readonly IOptionsSnapshot<RetryConfigure> _options;

        public RetryMiddleware(IClock clock, IOptionsSnapshot<RetryConfigure> options)
        {
            _clock = clock;
            _options = options;
        }

        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request,
                                                        IPipelineContext context,
                                                        Func<Task<HttpResponseMessage>> next,
                                                        CancellationToken cancellationToken)
        {
            var retryConfigure = _options.Get(request.ClientName);

            for (var i = 0; i < retryConfigure.RetryCount; i++)
            {
                var response = await next();

                if (response.IsSuccessStatusCode)
                    return response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Don't reattempt a bad request
                    return response;
                }

                if (retryConfigure.SleepDurationRetry > TimeSpan.Zero)
                    await _clock.SleepAsync(retryConfigure.SleepDurationRetry , cancellationToken);
            }

            throw new RetryOutOfRangeException();
        }
    }
}