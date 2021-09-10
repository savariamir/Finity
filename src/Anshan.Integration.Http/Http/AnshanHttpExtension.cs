using System;
using System.Net.Http;
using Anshan.Integration.Http.Http.Configuration;
using Anshan.Integration.Http.Http.Retry;
using Microsoft.Extensions.DependencyInjection;

namespace Anshan.Integration.Http.Http
{
    public static class AnshanHttpExtension
    {
        public static IHttpClientBuilder AddAnshanHttpClient(this IServiceCollection services, string name,
            Action<HttpClient> action)
        {
            var builder = services.AddHttpClient(name, action);

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
            
            builder.AddHttpMessageHandler<RetryHandler>()
                .ConfigureHttpClient(retryConfigure);

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