using Microsoft.Extensions.DependencyInjection;

namespace Shemy.Prometheus
{
    public static class ShemyHttpExtension
    {
        public static IHttpClientBuilder AddPrometheus(this IHttpClientBuilder builder)
        {
            return builder;
        }
    }
}