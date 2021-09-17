using Microsoft.Extensions.DependencyInjection;
using Shemy.Extension;
using Shemy.Pipeline;

namespace Shemy.Prometheus
{
    public static class ShemyHttpExtension
    {
        public static IShemyHttpClientBuilder AddPrometheus(this IShemyHttpClientBuilder builder)
        {
            builder.Services.AddTransient<MetricMiddleware>();
            builder.Services.AddTransient<IMetricProxy, MetricProxy>();
            
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MetricMiddleware)));
            
            return builder;
        }
    }
}