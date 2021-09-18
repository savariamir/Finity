using System;
using Finity.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Bulkhead
{
    public static class BulkheadExtension
    {
        public static IHttpClientBuilder Bulkhead(this IHttpClientBuilder builder,
            Action<BulkheadConfigure> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            
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