using Microsoft.Extensions.DependencyInjection;
using Shemy.Pipeline.Abstractions;
using Shemy.Pipeline.Internal;

namespace Shemy.Pipeline
{
    public static class ServiceCollectionExtensions
    {
        public static PipelineBuilder<TRequest, TResponse> AddPipeline<TRequest, TResponse>(
            this IServiceCollection services, string name)
        {
            var builder = new PipelineBuilder<TRequest, TResponse>(services);

            services.AddTransient<IPipeline<TRequest, TResponse>>(sp =>
                new LazyPipeline<TRequest, TResponse>(sp, builder.Middlewares, name));

            return builder;
        }
    }
}