using System;
using Microsoft.Extensions.DependencyInjection;
using Shemy.Pipeline;

namespace Shemy.Caching
{
    public static class CacheExtension
    {
        public static IHttpClientBuilder Cache(this IHttpClientBuilder builder,
            Action<CacheConfigure> cacheConfigure)
        {
            builder.Services.AddTransient<MemoryCacheMiddleware>();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(MemoryCacheMiddleware)));
            builder.Services.Configure(builder.Name, cacheConfigure);
            return builder;
        }
    }
}