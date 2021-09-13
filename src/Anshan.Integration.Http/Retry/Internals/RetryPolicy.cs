using Anshan.Integration.Http.Retry.Abstractions;
using Anshan.Integration.Http.Retry.Configurations;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Retry.Internals
{
    internal class RetryPolicy : IRetryPolicy
    {
        private int _retryCount;

        public RetryPolicy(IOptions<RetryConfigure> options)
        {
            _retryCount = options.Value.RetryCount;
        }

        public bool CanRetry()
        {
            var canRetry = _retryCount > 0;
            _retryCount -= 1;

            return canRetry;
        }
    }
}