using System;
using System.Net.Http;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Http.Caching;
using Anshan.Integration.Http.Http.Configuration;
using Anshan.Integration.Http.Http.Retry;
using Microsoft.Extensions.DependencyInjection;

namespace Anshan.Integration.Http.Http
{
    public static class AnshanHttpExtension
    {
        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name)
        {
            var builder = services.AddHttpClient(name);

            services.AddTransient<IAnshanHttp, AnshanHttp>();
            services.AddTransient<IClock, SystemClock>();

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

            builder.Services.AddTransient<RetryHandler>();
            builder.Services.AddTransient<IRetryEngine, RetryEngine>();
            builder.Services.AddTransient<IRetryPolicy, RetryPolicy>();
            builder.Services.AddMemoryCache();

            builder.AddHttpMessageHandler<RetryHandler>()
                   .ConfigureHttpClient(retryConfigure);

            return builder;
        }

        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder)
        {
            builder.Services.AddTransient<CacheHandler>();
            builder.AddHttpMessageHandler<CacheHandler>();
            builder.Services.AddMemoryCache();

            return builder;
        }
        
        public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

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

            builder.Services.Configure(retryConfigure);

            return builder;
        }
        
        private static IHttpClientBuilder ConfigureHttpClient(this IHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> retryConfigure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (retryConfigure == null)
            {
                throw new ArgumentNullException(nameof(retryConfigure));
            }

            builder.Services.Configure(retryConfigure);

            return builder;
        }
    }
}