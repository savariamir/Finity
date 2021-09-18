using System;
using System.Collections.Generic;
using System.Net.Http;
using Finity.Authentication;
using Finity.Bulkhead;
using Finity.Caching;
using Finity.CircuitBreaker.Configurations;
using Finity.CircuitBreaker.Extensions;
using Finity.CircuitBreaker.Internals;
using Finity.Default;
using Finity.Pipeline;
using Finity.Pipeline.Internal;
using Finity.Request;
using Finity.Retry.Configurations;
using Finity.Retry.Extensions;
using Finity.Retry.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Extension
{
    public static class ShemyHttpExtension
    {
        public static IShemyHttpClientBuilder AddShemyHttpClient(
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

            return new ShemyHttpClientBuilder(builder.Name, builder.Services);
        }

        public static IShemyHttpClientBuilder AddShemyHttpClient(
            this IServiceCollection services, string name = "")
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

            return new ShemyHttpClientBuilder(builder.Name, builder.Services);
        }


        public static IShemyHttpClientBuilder AddRetry(this IShemyHttpClientBuilder builder,
            Action<RetryConfigure> configure)
        {
            builder.Retry(configure);

            return builder;
        }


        public static IShemyHttpClientBuilder AddCache(this IShemyHttpClientBuilder builder,
            Action<CacheConfigure> configure)
        {
            builder.Cache(configure);
            return builder;
        }

        public static IShemyHttpClientBuilder AddCircuitBreaker(this IShemyHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
            builder.CircuitBreaker(configure);
            return builder;
        }

        public static IShemyHttpClientBuilder AddBulkhead(this IShemyHttpClientBuilder builder,
            Action<BulkheadConfigure> configure)
        {
            builder.Bulkhead(configure);

            return builder;
        }

        public static IShemyHttpClientBuilder AddAuthentication(
            this IShemyHttpClientBuilder builder,
            Action<AuthenticationConfigure> configure)
        {
            builder.Authentication(configure);

            return builder;
        }
    }
}