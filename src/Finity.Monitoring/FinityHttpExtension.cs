using System;
using Finity.Extensions;
using Finity.Shared;
using Microsoft.Extensions.DependencyInjection;
namespace Finity.Monitoring
{
    public static class FinityHttpExtension
    {
        public static IHttpClientBuilder AddPrometheus(this IHttpClientBuilder builder)
        {
            Action<MetricValue> setMetric = new MetricHandler().SetMetric;
            builder.TryAddFinity(setMetric);
            return builder;
        }
    }
}