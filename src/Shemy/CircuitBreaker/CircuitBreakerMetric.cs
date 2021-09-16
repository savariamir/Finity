using System;
using System.Collections.Concurrent;
using Shemy.Clock;

namespace Shemy.CircuitBreaker
{
    internal class CircuitBreakerMetric : ICircuitBreakerMetric
    {
        private readonly ConcurrentDictionary<string, MetricItem> _metrics = new();
        private readonly IClock _clock;

        public CircuitBreakerMetric(IClock clock)
        {
            _clock = clock;
        }

        public void SetState(CircuitBreakerState state, string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                var newValue = new MetricItem
                {
                    Failures = value.Failures,
                    Successes = value.Successes,
                    LastFailureDateTimeUtc =value.LastFailureDateTimeUtc,
                    CircuitBreakerState = state
                };
                _metrics.TryUpdate(name, value, newValue);
            }
        }

        public void IncrementSuccess(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                var newValue = new MetricItem
                {
                    Failures = value.Failures,
                    Successes = value.Successes + 1,
                    LastFailureDateTimeUtc = value.LastFailureDateTimeUtc
                };
                _metrics.TryUpdate(name, value, newValue);
            }
        }

        public void IncrementFailure(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                var newValue = new MetricItem
                {
                    Failures = value.Failures + 1,
                    Successes = value.Successes,
                    LastFailureDateTimeUtc = _clock.UtcNow()
                };
                _metrics.TryUpdate(name, value, newValue);
            }
        }

        public int GetFailures(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                return value.Failures;
            }
        }

        public int GetSuccess(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                return value.Successes;
            }
        }

        public DateTime GetLastFailureDateTimeUtc(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                return value.LastFailureDateTimeUtc;
            }
        }

        public void Reset(string name)
        {
            lock (name)
            {
                _metrics.TryRemove(name, out _);
            }
        }

        public CircuitBreakerState GetState(string name)
        {
            lock (name)
            {
                var value = _metrics.GetOrAdd(name, key => new MetricItem());
                return value.CircuitBreakerState;
            }
        }
    }
}