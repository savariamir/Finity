using Anshan.Integration.Http.Http.Configuration;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http.Http.Retry
{
    public class RetryPolicy : IRetryPolicy
    {
        private int _retryCount;

        public RetryPolicy(IOptions<RetryConfigure> options)
        {
            _retryCount = options.Value.RetryCount;
        }

        public bool CanRetry()
        {
            return _retryCount > 0;
        }
    }
}