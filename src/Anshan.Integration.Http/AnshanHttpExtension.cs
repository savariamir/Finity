using System;
using System.Net.Http;
using Anshan.Integration.Http.Caching;
using Anshan.Integration.Http.CircuitBreaker;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Configuration;
using Anshan.Integration.Http.Retry.Abstractions;
using Anshan.Integration.Http.Retry.Configurations;
using Anshan.Integration.Http.Retry.Internals;
using EasyPipe.Extensions.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Anshan.Integration.Http
{
    public static class AnshanHttpExtension
    {
        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name)
        {
            services.AddPipeline<AnshanHttpRequestMessage, HttpResponseMessage>()
                    .WithMiddleware<MemoryCacheMiddleware>()
                    .WithMiddleware<RetryMiddleware>()
                    .WithMiddleware<CircuitBreakerMiddleware>()
                    .WithMiddleware<HttpMiddleware>();

            var builder = services.AddHttpClient(name);

            services.AddTransient<DefaultDelegationHandler>();
            builder.AddHttpMessageHandler<DefaultDelegationHandler>();
            builder.Services.AddMemoryCache();

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
            builder.Services.AddTransient<IRetryPolicy, RetryPolicy>();
            builder.Services.AddMemoryCache();

            builder.ConfigureHttpClient(retryConfigure);

            builder.Services.Configure<AnshanFactoryOptions>(builder.Name, options => options.IsRetryEnabled = true);
            


            return builder;
        }

        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder,
                                                  Action<CacheConfigure> cacheConfigure)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<ICacheEngine, CacheEngine>();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name, options => options.IsCacheEnabled = true);
            builder.Services.Configure(cacheConfigure);
            return builder;
        }

        public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<ICircuitBreakerEngine, CircuitBreakerEngine>();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.IsCircuitBreakerEnabled = true);
            return builder;
        }


        private static IHttpClientBuilder ConfigureHttpClient(this IHttpClientBuilder builder,
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

            builder.Services.Configure(builder.Name, retryConfigure);

            return builder;
        }
    }
}