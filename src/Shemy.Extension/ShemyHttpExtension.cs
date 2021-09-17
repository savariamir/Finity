using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Authentication;
using Shemy.Bulkhead;
using Shemy.Caching;
using Shemy.CircuitBreaker.Configurations;
using Shemy.CircuitBreaker.Extensions;
using Shemy.CircuitBreaker.Internals;
using Shemy.Default;
using Shemy.Pipeline;
using Shemy.Pipeline.Internal;
using Shemy.Request;
using Shemy.Retry;
using Shemy.Retry.Configurations;
using Shemy.Retry.Internals;

namespace Shemy.Extension
{
    public static class ShemyHttpExtension
    {
        public static IHttpClientBuilder AddShemyHttpClient(
            this IServiceCollection services, string name,
            Action<HttpClient> configureClient)
        {
            services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
                typeof(AuthenticationMiddleware),
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

        public static IHttpClientBuilder AddShemyHttpClient(this IServiceCollection services, string name = "")
        {
            services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
                typeof(AuthenticationMiddleware),
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
            Action<RetryConfigure> configure)
        {
            builder.Retry(configure);

            return builder;
        }


        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder,
            Action<CacheConfigure> configure)
        {
            builder.Cache(configure);
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
            builder.Bulkhead(configure);

            return builder;
        }
    }
}