using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.Clock;
using Shemy.Extensions;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;
using Shemy.Retry.Configurations;
using Shemy.Retry.Exceptions;

namespace Shemy.Retry.Internals
{
    public class RetryMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
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

                if (response.IsSucceed())
                    return response;

                if (retryConfigure.SleepDurationRetry > TimeSpan.Zero)
                    await _clock.SleepAsync(retryConfigure.SleepDurationRetry , cancellationToken);
            }

            throw new RetryOutOfRangeException();
        }
    }
}