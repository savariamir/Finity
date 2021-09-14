using Microsoft.Extensions.DependencyInjection;
using Shemy.Pipeline.Abstractions;
using Shemy.Pipeline.Internal;

namespace Shemy.Pipeline
{
    public static class ServiceCollectionExtensions
    {
        public static PipelineBuilder<TRequest, TResponse> AddPipeline<TRequest, TResponse>(this IServiceCollection services)
        {
            var builder = new PipelineBuilder<TRequest, TResponse>(services);

            services.AddTransient<IPipeline<TRequest, TResponse>>(sp =>
            {
                return new LazyPipeline<TRequest, TResponse>(sp, builder.Middlewares);
            });
            
            return builder;
        } 
        
        public static PipelineBuilder<TResponse> AddPipeline<TResponse>(this IServiceCollection services)
        {
            var builder = new PipelineBuilder<TResponse>(services);

            services.AddTransient<IPipeline<TResponse>>(sp =>
            {
                return new LazyPipeline<TResponse>(sp, builder.Middlewares);
            });
            
            return builder;
        } 
    }
}