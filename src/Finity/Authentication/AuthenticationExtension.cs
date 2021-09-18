using System;
using Finity.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.Authentication
{
    public static class AuthenticationExtension
    {
        public static IHttpClientBuilder Authentication(
            this IHttpClientBuilder builder,
            Action<AuthenticationConfigure> configure)
        {
            if (builder is  null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.Services.AddTransient<AuthenticationMiddleware>();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(AuthenticationMiddleware)));
            builder.Services.Configure(builder.Name, configure);

            return builder;
        }
    }
}