using System;
using System.Collections.Concurrent;

namespace Shemy.CircuitBreaker
{
    public class CircuitBreakerMetric : ICircuitBreakerMetric
    {
        private readonly ConcurrentDictionary<string, MetricItem> _metrics = new();

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
                    LastFailureDateTimeUtc = DateTime.UtcNow
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

    public interface ICircuitBreakerMetric
    {
        void SetState(CircuitBreakerState state,string name);
        void IncrementSuccess(string name);
        void IncrementFailure(string name);
        int GetFailures(string name);
        int GetSuccess(string name);
        DateTime GetLastFailureDateTimeUtc(string name);
        void Reset(string name);
        CircuitBreakerState GetState(string name);
    }

    public class MetricItem
    {
        public int Successes { get; set; }

        public int Failures { get; set; }

        public int Total
        {
            get { return Successes + Failures; }
        }

        public CircuitBreakerState CircuitBreakerState { get; set; } = CircuitBreakerState.Closed;

        public DateTime LastFailureDateTimeUtc { get; set; }
    }
}