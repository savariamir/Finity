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
        private static IHttpClientBuilder TryAddFinity(this IHttpClientBuilder builder)
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

            return builder;
        }

        public static IHttpClientBuilder WithRetry(this IHttpClientBuilder builder,
                                                   Action<RetryConfigure> configure)
        {
            builder.TryAddFinity();
            builder.Retry(configure);

            return builder;
        }


        public static IHttpClientBuilder WithCache(this IHttpClientBuilder builder,
                                                   Action<CacheConfigure> configure)
        {
            builder.TryAddFinity();
            builder.Cache(configure);
            return builder;
        }

        public static IHttpClientBuilder WithCircuitBreaker(this IHttpClientBuilder builder,
                                                            Action<CircuitBreakerConfigure> configure)
        {
            builder.TryAddFinity();
            builder.CircuitBreaker(configure);
            return builder;
        }

        public static IHttpClientBuilder WithBulkhead(this IHttpClientBuilder builder,
                                                      Action<BulkheadConfigure> configure)
        {
            builder.TryAddFinity();
            builder.Bulkhead(configure);
            return builder;
        }

        public static IHttpClientBuilder AddAuthentication(
            this IHttpClientBuilder builder,
            Action<AuthenticationConfigure> configure)
        {
            builder.TryAddFinity();
            builder.Authentication(configure);
            return builder;
        }
    }
}