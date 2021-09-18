using System;

namespace Finity.Retry.Configurations
{
    public class RetryConfigure
    {
        public int RetryCount { get; set; }
        public TimeSpan SleepDurationRetry { get; set; } = TimeSpan.Zero;
    }
}