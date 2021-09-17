using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shemy.Clock
{
    public class SystemClock : IClock
    {
        /// <summary>
        /// Allows the setting of a custom DateTime.UtcNow implementation for testing.
        /// By default this will be a call to <see cref="DateTime.UtcNow"/>
        /// </summary>
        public DateTime UtcNow() => DateTime.UtcNow;

        /// <summary>
        /// Allows the setting of a custom async Sleep implementation for testing.
        /// By default this will be a call to <see cref="M:Task.Delay"/> taking a <see cref="CancellationToken"/>
        /// </summary>
        public async Task SleepAsync(TimeSpan timeSpan, CancellationToken cancellationToken) =>
            await Task.Delay(timeSpan, cancellationToken);
    }
}