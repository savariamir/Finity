using System;

namespace Shemy.Retry.Configurations
{
    public class RetryConfigure
    {
        public int RetryCount { get; set; }
        public TimeSpan SleepDurationRetry { get; set; } = TimeSpan.Zero;
    }
}