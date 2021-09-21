using System;
using Finity.Shared.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Prometheus
{
    public static class FinityHttpExtension
    {
        public static IHttpClientBuilder WithPrometheus(this IHttpClientBuilder builder)
        {
            Action<MetricValue> setMetric = new MetricHandler().SetMetric;
            builder.TryAddFinity(setMetric);
            
            builder.Services.AddSingleton<MetricHandler>();
            return builder;
        }
    }
}