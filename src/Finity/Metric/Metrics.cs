using System.Collections.Concurrent;

namespace Finity.Metric
{
    public static class Metrics
    {
        public static ConcurrentDictionary<string, MetricValue> Items = new();
        private const string Suffix = "dotnet_finity_";
        public static readonly string CircuitBreakerOpenedCount = $"{Suffix}circuit_breaker_opened_count";
        public static readonly string CircuitBreakerClosedCount = $"{Suffix}circuit_breaker_closed_count";
        public static readonly string TotalNumberOfExecutions = $"{Suffix}total_number_of_executions";
        public static readonly string TotalNumberOfSuccessfulExecutions = $"{Suffix}total_number_of_successful_executions";
        public static readonly string TotalNumberOfFailureExecutions = $"{Suffix}total_number_of_failure_executions";
        public static readonly string AverageExecutionTimeMilliseconds = $"{Suffix}average_execution_time_milliseconds";
        public static readonly string FirstTryCount = $"{Suffix}first_try_count";
        public static readonly string NextTryCount = $"{Suffix}next_try_count";

        public static void TryAdd(string name, GaugeValue value)
        {
        }
        
        public static void Increment(string name)
        {
        }
    }


    public class MetricValue
    {
    }
    
    public class CounterValue : MetricValue
    {
    }
    
    public class GaugeValue : MetricValue
    {
    }
    
}