using System;
using Finity.CircuitBreaker.Abstractions;
using Finity.CircuitBreaker.Configurations;
using Finity.CircuitBreaker.Internals;
using Finity.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Finity.CircuitBreaker.Extensions
{
    public static class CircuitBreakerExtension
    {
        public static IHttpClientBuilder CircuitBreaker(
            this IHttpClientBuilder builder,
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
            
            builder.Services.AddTransient<CircuitBreakerMiddleware>();
            builder.Services.AddTransient<ICircuitBreakerEngine, CircuitBreakerEngine>();
            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => { options.Types.Add(typeof(CircuitBreakerMiddleware)); });

            builder.Services.AddSingleton<ICircuitBreakerLockProvider>(_ =>
                new CircuitBreakerLockProvider(builder.Name));
            
            builder.Services.AddSingleton<ICircuitBreakerStateProvider>(_ =>
                new CircuitBreakerStateProvider(builder.Name));

            builder.Services.AddSingleton<ICircuitBreakerMetric, CircuitBreakerMetric>();
            builder.Services.Configure(builder.Name, configure);

            return builder;
        }
    }
}