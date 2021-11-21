using System;
using Finity.Caching.Abstractions;
using Finity.Caching.Configurations;
using Finity.Caching.Internals;
using Finity.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Caching.Extensions
{
    public static class CacheExtension
    {
        public static IHttpClientBuilder Cache(this IHttpClientBuilder builder,
                                               Action<CacheConfigure> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.Services.AddTransient<MemoryCacheMiddleware>();
            builder.Services.AddTransient<IMemoryCacheProvider, MemoryCacheProvider>();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MemoryCacheMiddleware)));
            builder.Services.Configure(builder.Name, configure);
            return builder;
        }
    }
}