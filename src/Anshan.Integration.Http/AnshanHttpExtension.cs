using System;
using System.Net.Http;
using Anshan.Integration.Http.Caching;
using Anshan.Integration.Http.Clock;
using Anshan.Integration.Http.Configuration;
using Anshan.Integration.Http.Retry.Appstractions;
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
                    .WithMiddleware<InMemoryCacheMiddleware>()
                    .WithMiddleware<RetryMiddleware>()
                    .WithMiddleware<HttpMiddleware>();

            var builder = services.AddHttpClient(name);

            services.AddTransient<IClock, SystemClock>();
            services.AddTransient<IHttpEngine, HttpEngine>();
            services.AddTransient<HttpDelegationHandler>();
            builder.AddHttpMessageHandler<HttpDelegationHandler>();
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

            builder.Services.AddTransient<IRetryEngine, RetryEngine>();
            builder.Services.AddTransient<IRetryPolicy, RetryPolicy>();
            builder.Services.AddMemoryCache();

            builder.ConfigureHttpClient(retryConfigure);

            builder.Services.Configure<AnshanFactoryOptions>(options => options.IsRetryEnabled = true);

            builder.Services.Configure(retryConfigure);


            return builder;
        }

        public static IHttpClientBuilder AddCache(this IHttpClientBuilder builder, Action<CacheConfigure> cacheConfigure)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<ICacheEngine, CacheEngine>();
            builder.Services.Configure<AnshanFactoryOptions>( options => options.IsCacheEnabled = true);
            builder.Services.Configure(cacheConfigure);
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
    }
}