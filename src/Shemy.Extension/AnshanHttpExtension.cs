using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Bulkhead;
using Shemy.Caching;
using Shemy.CircuitBreaker.Configurations;
using Shemy.CircuitBreaker.Extensions;
using Shemy.CircuitBreaker.Internals;
using Shemy.Clock;
using Shemy.Default;
using Shemy.Pipeline;
using Shemy.Pipeline.Internal;
using Shemy.Request;
using Shemy.Retry.Configurations;
using Shemy.Retry.Internals;

namespace Shemy.Extension
{
    public static class AnshanHttpExtension
    {
        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name,
            Action<HttpClient> configureClient)
        {
            services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
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
            services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
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
            builder.Services.AddTransient<RetryMiddleware>();
            builder.Services.Configure(builder.Name, retryConfigure);
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(RetryMiddleware)));

            return builder;
        }


        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder,
            Action<CacheConfigure> cacheConfigure)
        {
            builder.Services.AddTransient<MemoryCacheMiddleware>();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MemoryCacheMiddleware)));
            builder.Services.Configure(builder.Name, cacheConfigure);
            return builder;
        }

        public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
            builder.CircuitBreaker(configure);
            return builder;
        }

        public static IHttpClientBuilder AddBulkhead(this IHttpClientBuilder builder,
            Action<BulkheadConfigure> configure)
        {
            builder.Services.AddTransient<BulkheadMiddleware>();

            builder.Services.AddSingleton<IBulkheadLockProvider>(_ =>
                new BulkheadLockProvider(builder.Name, 2));

            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => { options.Types.Add(typeof(BulkheadMiddleware)); });


            builder.Services.Configure(builder.Name, configure);

            return builder;
        }
    }
}