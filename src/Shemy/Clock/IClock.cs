using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shemy.Clock
{
    internal interface IClock
    {
        // Func<TimeSpan, CancellationToken, Task> SleepAsync ;
        /// <summary>
        /// Allows the setting of a custom async Sleep implementation for testing.
        /// By default this will be a call to <see cref="M:Task.Delay"/> taking a <see cref="CancellationToken"/>
        /// </summary>
        Task SleepAsync(TimeSpan timeSpan, CancellationToken cancellationToken);

        public DateTime UtcNow();
    }
}