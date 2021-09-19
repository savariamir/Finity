using System;
using System.Collections.Concurrent;
using Prometheus;

namespace Finity.Monitoring
{
    public class MetricReporter
    {
        private readonly ConcurrentDictionary<string, Counter> _counters = new();
        private readonly ConcurrentDictionary<string, Gauge> _gauges = new();
        private readonly ConcurrentDictionary<string, Gauge> _lastExecutionDateTime = new();

        public void Report(string name, int millisecond)
        {
            RegisterAverageRunning(name, millisecond);
            RegisterRunningWorker(name);
            RegisterLastExecution(name);
        }


        private void RegisterLastExecution(string name)
        {
            if (!_lastExecutionDateTime.TryGetValue(name, out var value))
            {
                value = Metrics.CreateGauge($"last_execution_time_of_{name}",
                    "The average execution time of the worker in milliseconds");
                _lastExecutionDateTime.TryAdd(name, value);
            }

            value.SetToCurrentTimeUtc();
        }

        private void RegisterRunningWorker(string name)
        {
            if (!_counters.TryGetValue(name, out var value))
            {
                value = Metrics.CreateCounter($"total_number_of_{name}_executions",
                    "The total number of the worker executions");
                _counters.TryAdd(name, value);
            }

            value.Inc();
        }


        private void RegisterAverageRunning(string name, int millisecond)
        {
            if (!_gauges.TryGetValue(name, out var value))
            {
                value = Metrics.CreateGauge($"average_execution_time_of_{name}_milliseconds",
                    "The average execution time of the worker in milliseconds");
                _gauges.TryAdd(name, value);
            }

            var result = Math.Ceiling((value.Value * _counters.Count + millisecond) / (_counters.Count + 1));

            value.Set(result);
        }
    }
}