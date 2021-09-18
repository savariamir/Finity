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
    public static class FinityHttpExtension
    {
        public static IFinityHttpClientBuilder AddFinity(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
                typeof(AuthenticationMiddleware),
                typeof(DefaultMiddleware),
            };

            
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(DefaultMiddleware)));

            builder.AddHttpMessageHandler((sp) =>
            {
                var pipeline =
                    new LazyPipeline<AnshanHttpRequestMessage, HttpResponseMessage>(sp, middlewares, builder.Name);
                return new DefaultDelegationHandler(builder.Name, pipeline);
            });

            return new FinityHttpClientBuilder(builder.Name, builder.Services);
        }

        public static IFinityHttpClientBuilder WithRetry(this IFinityHttpClientBuilder builder,
            Action<RetryConfigure> configure)
        {
            builder.Retry(configure);

            return builder;
        }


        public static IFinityHttpClientBuilder WithCache(this IFinityHttpClientBuilder builder,
            Action<CacheConfigure> configure)
        {
            builder.Cache(configure);
            return builder;
        }

        public static IFinityHttpClientBuilder WithCircuitBreaker(this IFinityHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
            builder.CircuitBreaker(configure);
            return builder;
        }

        public static IFinityHttpClientBuilder WithBulkhead(this IFinityHttpClientBuilder builder,
            Action<BulkheadConfigure> configure)
        {
            builder.Bulkhead(configure);

            return builder;
        }

        public static IFinityHttpClientBuilder AddAuthentication(
            this IFinityHttpClientBuilder builder,
            Action<AuthenticationConfigure> configure)
        {
            builder.Authentication(configure);

            return builder;
        }
    }
}