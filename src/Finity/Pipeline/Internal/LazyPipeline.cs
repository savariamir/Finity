using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Pipeline.Exceptions;
using Finity.Shared;
using Finity.Shared.Metrics;
using Microsoft.Extensions.Options;

namespace Finity.Pipeline.Internal
{
    public class LazyPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse>
    {
        private readonly Type[] _middlewareTypes;
        private readonly IServiceProvider _serviceProvider;
        private readonly Action<MetricValue> _setMetric;
        private readonly AnshanFactoryOptions _options;

        public LazyPipeline(IServiceProvider serviceProvider, IEnumerable<Type> middlewareTypes,
            Action<MetricValue> setMetric, string clientName)
        {
            _serviceProvider = serviceProvider;
            _setMetric = setMetric;
            _middlewareTypes = middlewareTypes.ToArray();
            _options = ((IOptionsSnapshot<AnshanFactoryOptions>)
                _serviceProvider.GetService(typeof(IOptionsSnapshot<AnshanFactoryOptions>)))?.Get(clientName);
        }

        public Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken)
        {
            IPipelineContext context = new PipelineContext();

            var middlewares = _middlewareTypes.Where(middlewareType =>
                    _options.Types.Contains(middlewareType))
                .ToArray();

            Task<TResponse> Next(Type? type)
            {
                Type middlewareType = type is null ? middlewares.First() : middlewares.First(p => p == type);

                if (type is not null)
                {
                    var nextIndexType = Array.IndexOf(middlewares, middlewareType) + 1;
                    if (nextIndexType >= middlewares.Length)
                    {
                        return Task.FromResult<TResponse>(default);
                    }

                    middlewareType = middlewares[nextIndexType];
                }

                if (_serviceProvider.GetService(middlewareType) is not IMiddleware<TRequest, TResponse> middleware)
                {
                    throw new MiddlewareNotResolvedException(middlewareType);
                }

                return middleware.ExecuteAsync(request, context, Next, _setMetric, cancellationToken);
            }

            return Next(null);
        }
    }
}