using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Caching;
using Shemy.CircuitBreaker;
using Shemy.Clock;
using Shemy.Default;
using Shemy.Pipeline;
using Shemy.Pipeline.Internal;
using Shemy.Request;
using Shemy.Retry.Configurations;
using Shemy.Retry.Internals;

namespace Shemy.Extensions
{
    public static class AnshanHttpExtension
    {
        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name,
                                                             Action<HttpClient> configureClient)
        {
            services.AddTransient<MemoryCacheMiddleware>();
            services.AddTransient<CircuitBreakerMiddleware>();
            services.AddTransient<RetryMiddleware>();
            services.AddTransient<DefaultMiddleware>();


            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(DefaultMiddleware),
            };


            var builder = services.AddHttpClient(name, configureClient);
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(DefaultMiddleware)));
            
            builder.AddHttpMessageHandler((sp) =>
            {
                var pipeline =
                    new LazyPipeline<AnshanHttpRequestMessage, HttpResponseMessage>(sp, middlewares, name);
                return new DefaultDelegationHandler(name, pipeline);
            });

            return builder;
        }

        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name = "")
        {
            services.AddTransient<MemoryCacheMiddleware>();
            services.AddTransient<CircuitBreakerMiddleware>();
            services.AddTransient<RetryMiddleware>();
            services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(DefaultMiddleware),
            };


            var builder = services.AddHttpClient(name);
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(DefaultMiddleware)));
            
            
            builder.AddHttpMessageHandler((sp) =>
            {
                var pipeline =
                    new LazyPipeline<AnshanHttpRequestMessage, HttpResponseMessage>(sp, middlewares, name);
                return new DefaultDelegationHandler(name, pipeline);
            });

            return builder;
        }


        public static IHttpClientBuilder AddRetry(this IHttpClientBuilder builder,
                                                  Action<RetryConfigure> retryConfigure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (retryConfigure == null)
            {
                throw new ArgumentNullException(nameof(retryConfigure));
            }


            builder.Services.AddTransient<IClock, SystemClock>();


            builder.Services.Configure(builder.Name, retryConfigure);


            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(RetryMiddleware)));

            return builder;
        }


        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder,
                                                  Action<CacheConfigure> cacheConfigure)
        {
            builder.Services.AddMemoryCache();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MemoryCacheMiddleware)));
            builder.Services.Configure(builder.Name, cacheConfigure);
            return builder;
        }

        public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<ICircuitBreakerEngine, CircuitBreakerEngine>();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(CircuitBreakerMiddleware)));
            return builder;
        }
    }
}