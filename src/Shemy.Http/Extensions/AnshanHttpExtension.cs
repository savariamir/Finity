using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Http.Caching;
using Shemy.Http.CircuitBreaker;
using Shemy.Http.Clock;
using Shemy.Http.Configuration;
using Shemy.Http.Default;
using Shemy.Http.Request;
using Shemy.Http.Retry.Configurations;
using Shemy.Http.Retry.Internals;
using Shemy.Pipeline;
using Shemy.Pipeline.Internal;

namespace Shemy.Http.Extensions
{
    public interface IAnshanHttpClientBuilder
    {
    }

    internal class AnshanHttpClientBuilder : IAnshanHttpClientBuilder
    {
        public PipelineBuilder<AnshanHttpRequestMessage, HttpResponseMessage> PipelineBuilder { get; set; }
        public IServiceCollection Services { get; set; }
        public IHttpClientBuilder HttpClientBuilder { get; }

        public string Name { get; set; }

        public AnshanHttpClientBuilder(IHttpClientBuilder httpClientBuilder)
        {
            HttpClientBuilder = httpClientBuilder;
        }
    }

    public static class AnshanHttpExtension
    {
        public static IAnshanHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name,
                                                                   Action<HttpClient> configureClient)
        {
            var builder = services.AddHttpClient(name, configureClient);


            var anshanHttpClientBuilder = new AnshanHttpClientBuilder(builder)
            {
                PipelineBuilder = new PipelineBuilder<AnshanHttpRequestMessage, HttpResponseMessage>(services),
                Services = services,
                Name = name
            };

            anshanHttpClientBuilder.PipelineBuilder.AddMiddleware<DefaultMiddleware>();

            builder.AddHttpMessageHandler(sp =>
            {
                anshanHttpClientBuilder.PipelineBuilder.Middlewares.Reverse();
                var pipeline = new LazyPipeline<AnshanHttpRequestMessage,
                    HttpResponseMessage>(sp, anshanHttpClientBuilder.PipelineBuilder.Middlewares);

                return new DefaultDelegationHandler(name, pipeline);
            });

            return anshanHttpClientBuilder;
        }
        
        public static IAnshanHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name)
        {
            var builder = services.AddHttpClient(name);


            var anshanHttpClientBuilder = new AnshanHttpClientBuilder(builder)
            {
                PipelineBuilder = new PipelineBuilder<AnshanHttpRequestMessage, HttpResponseMessage>(services),
                Services = services,
                Name = name
            };

            anshanHttpClientBuilder.PipelineBuilder.AddMiddleware<DefaultMiddleware>();

            builder.AddHttpMessageHandler(sp =>
            {
                anshanHttpClientBuilder.PipelineBuilder.Middlewares.Reverse();
                var pipeline = new LazyPipeline<AnshanHttpRequestMessage,
                    HttpResponseMessage>(sp, anshanHttpClientBuilder.PipelineBuilder.Middlewares);

                return new DefaultDelegationHandler(name, pipeline);
            });

            return anshanHttpClientBuilder;
        }
        
        public static IAnshanHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services)
        {
            var builder = services.AddHttpClient(string.Empty);


            var anshanHttpClientBuilder = new AnshanHttpClientBuilder(builder)
            {
                PipelineBuilder = new PipelineBuilder<AnshanHttpRequestMessage, HttpResponseMessage>(services),
                Services = services,
                Name = string.Empty
            };

            anshanHttpClientBuilder.PipelineBuilder.AddMiddleware<DefaultMiddleware>();

            builder.AddHttpMessageHandler(sp =>
            {
                anshanHttpClientBuilder.PipelineBuilder.Middlewares.Reverse();
                var pipeline = new LazyPipeline<AnshanHttpRequestMessage,
                    HttpResponseMessage>(sp, anshanHttpClientBuilder.PipelineBuilder.Middlewares);

                return new DefaultDelegationHandler( string.Empty, pipeline);
            });

            return anshanHttpClientBuilder;
        }
        


        public static IAnshanHttpClientBuilder AddRetry(this IAnshanHttpClientBuilder anshanBuilder,
                                                        Action<RetryConfigure> retryConfigure)
        {
            var internalAnshanBuilder = anshanBuilder as AnshanHttpClientBuilder;

            internalAnshanBuilder.PipelineBuilder.AddMiddleware<RetryMiddleware>();

            if (anshanBuilder == null)
            {
                throw new ArgumentNullException(nameof(anshanBuilder));
            }

            if (retryConfigure == null)
            {
                throw new ArgumentNullException(nameof(retryConfigure));
            }


            internalAnshanBuilder.Services.AddTransient<IClock, SystemClock>();
            internalAnshanBuilder.Services.AddMemoryCache();

            internalAnshanBuilder.Services.Configure(internalAnshanBuilder.Name, retryConfigure);


            internalAnshanBuilder.Services.Configure<AnshanFactoryOptions>(internalAnshanBuilder.Name,
                factoryOptions => factoryOptions.IsRetryEnabled = true);

            return anshanBuilder;
        }


        public static IAnshanHttpClientBuilder AddCache(this IAnshanHttpClientBuilder builder,
                                                        Action<CacheConfigure> cacheConfigure)
        {
            var internalAnshanBuilder = builder as AnshanHttpClientBuilder;

            internalAnshanBuilder.PipelineBuilder.AddMiddleware<MemoryCacheMiddleware>();
            
            internalAnshanBuilder.Services.AddMemoryCache();
            internalAnshanBuilder.Services.Configure<AnshanFactoryOptions>(internalAnshanBuilder.Name,
                options => options.IsCacheEnabled = true);
            internalAnshanBuilder.Services.Configure(internalAnshanBuilder.Name, cacheConfigure);
            return builder;
        }

        public static IAnshanHttpClientBuilder AddCircuitBreaker(this IAnshanHttpClientBuilder builder)
        {
            var internalAnshanBuilder = builder as AnshanHttpClientBuilder;
            internalAnshanBuilder.PipelineBuilder.AddMiddleware<CircuitBreakerMiddleware>();
            internalAnshanBuilder.Services.AddTransient<ICircuitBreakerEngine, CircuitBreakerEngine>();
            internalAnshanBuilder.Services.Configure<AnshanFactoryOptions>(internalAnshanBuilder.Name,
                options => options.IsCircuitBreakerEnabled = true);
            return builder;
        }


        // private static IAnshanHttpClientBuilder ConfigureHttpClient(this IAnshanHttpClientBuilder builder,
        //                                                             Action<RetryConfigure> retryConfigure)
        // {
        //     if (builder == null)
        //     {
        //         throw new ArgumentNullException(nameof(builder));
        //     }
        //
        //     if (retryConfigure == null)
        //     {
        //         throw new ArgumentNullException(nameof(retryConfigure));
        //     }
        //
        //     builder.Services.Configure(builder.Name, retryConfigure);
        //
        //
        //     return builder;
        // }
    }
}