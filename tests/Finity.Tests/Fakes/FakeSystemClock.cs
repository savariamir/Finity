using System;
using System.Threading;
using System.Threading.Tasks;
using Finity.Clock;

namespace Finity.Tests.Fakes
{
    public class FakeSystemClock : IClock
    {
        public int SleepCallsCount;
        public Task SleepAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            SleepCallsCount++;
            return Task.CompletedTask;
        }

        public DateTime UtcNow()
        {
            return DateTime.Now;
        }
    }
}