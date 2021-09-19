using Finity.Extension;
using Finity.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Prometheus
{
    public static class ShemyHttpExtension
    {
        public static IHttpClientBuilder AddPrometheus(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<MetricMiddleware>();
            builder.Services.AddTransient<IMetricProxy, MetricProxy>();
            
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MetricMiddleware)));
            
            return builder;
        }
    }
}