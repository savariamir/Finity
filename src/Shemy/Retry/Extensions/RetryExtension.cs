using System;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Clock;
using Shemy.Pipeline;
using Shemy.Retry.Configurations;
using Shemy.Retry.Internals;

namespace Shemy.Retry.Extensions
{
    public static class RetryExtension
    {
        public static IHttpClientBuilder Retry(
            this IHttpClientBuilder builder,
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
    }
}