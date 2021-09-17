using System;
using Microsoft.Extensions.DependencyInjection;
using Shemy.CircuitBreaker.Abstractions;
using Shemy.CircuitBreaker.Configurations;
using Shemy.CircuitBreaker.Internals;
using Shemy.Pipeline;

namespace Shemy.CircuitBreaker.Extensions
{
    public static class CircuitBreakerExtension
    {
        public static IHttpClientBuilder CircuitBreaker(
            this IHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
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