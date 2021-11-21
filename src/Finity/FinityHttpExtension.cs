using System;
using System.Collections.Generic;
using System.Net.Http;
using Finity.Authentication;
using Finity.Bulkhead;
using Finity.Bulkhead.Configurations;
using Finity.Bulkhead.Extensions;
using Finity.Bulkhead.Internal;
using Finity.Bulkhead.Internals;
using Finity.Caching;
using Finity.Caching.Configurations;
using Finity.Caching.Extensions;
using Finity.Caching.Internals;
using Finity.CircuitBreaker.Configurations;
using Finity.CircuitBreaker.Extensions;
using Finity.CircuitBreaker.Internals;
using Finity.Default;
using Finity.Pipeline;
using Finity.Pipeline.Abstractions;
using Finity.Pipeline.Internal;
using Finity.Request;
using Finity.Retry.Configurations;
using Finity.Retry.Extensions;
using Finity.Retry.Internals;
using Finity.Shared;
using Finity.Shared.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Finity
{
    public static class FinityHttpExtension
    {
        private static Action<MetricValue>? _setGlobalMetric;

        public static IHttpClientBuilder WithRetry(this IHttpClientBuilder builder,
            Action<RetryConfigure> configure)
        {
            builder.TryAddFinity()
                .Retry(configure);

            return builder;
        }


        public static IHttpClientBuilder WithCache(this IHttpClientBuilder builder,
            Action<CacheConfigure> configure)
        {
            builder.TryAddFinity()
                .Cache(configure);
            return builder;
        }

        public static IHttpClientBuilder WithCircuitBreaker(this IHttpClientBuilder builder,
            Action<CircuitBreakerConfigure> configure)
        {
            builder.TryAddFinity()
                .CircuitBreaker(configure);
            return builder;
        }

        public static IHttpClientBuilder WithBulkhead(this IHttpClientBuilder builder,
            Action<BulkheadConfigure> configure)
        {
            builder
                .TryAddFinity()
                .Bulkhead(configure);
            return builder;
        }

        public static IHttpClientBuilder WithAuthentication(
            this IHttpClientBuilder builder,
            Action<AuthenticationConfigure> configure)
        {
            builder.TryAddFinity()
                .Authentication(configure);
            return builder;
        }

        public static IHttpClientBuilder TryAddFinity(this IHttpClientBuilder builder,
            Action<MetricValue> setInputMetric = null!)
        {
            builder.Services.AddTransient<DefaultMiddleware>();

            var middlewares = new List<Type>
            {
                typeof(MemoryCacheMiddleware),
                typeof(CircuitBreakerMiddleware),
                typeof(RetryMiddleware),
                typeof(BulkheadMiddleware),
                typeof(AuthenticationMiddleware),
                typeof(DefaultMiddleware),
            };

            Action<MetricValue> setMetric = (_) => { };
            var metric = setMetric;
            builder.Services.AddTransient<IMetricProvider>(_ => new MetricProvider(metric));

            builder.Services.AddTransient<IMetricProxy, MetricProxy>();


            if (setInputMetric is not null)
            {
                _setGlobalMetric = setInputMetric;
            }

            if (_setGlobalMetric is not null)
            {
                setMetric = _setGlobalMetric;
                builder.Services.AddTransient<IMetricProvider>(_ => new MetricProvider(_setGlobalMetric));
            }

            builder.Services.Configure<AnshanFactoryOptions>(builder.Name,
                options => options.Types.Add(typeof(DefaultMiddleware)));


            builder.AddHttpMessageHandler((sp) =>
            {
                var pipeline =
                    new LazyPipeline<FinityHttpRequestMessage, HttpResponseMessage>(
                        sp, middlewares, setMetric,
                        builder.Name);
                return new DefaultDelegationHandler(builder.Name, pipeline);
            });

            return builder;
        }

        private static IHttpClientBuilder AddHttpMessageHandler(
            this IHttpClientBuilder builder,
            Func<IServiceProvider, DelegatingHandler> configureHandler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureHandler == null)
            {
                throw new ArgumentNullException(nameof(configureHandler));
            }

            builder.Services.Configure<HttpClientFactoryOptions>(builder.Name,
                options =>
                {
                    options.HttpMessageHandlerBuilderActions.Clear();
                    options.HttpMessageHandlerBuilderActions.Add(b =>
                        b.AdditionalHandlers.Add(configureHandler(b.Services)));
                });

            return builder;
        }
    }
}