using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shemy.Pipeline.Abstractions;
using Shemy.Pipeline.Exceptions;

namespace Shemy.Pipeline.Internal
{
    public class LazyPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse>
    {
        private readonly Type[] _middlewareTypes;
        private readonly IServiceProvider _serviceProvider;
        private readonly AnshanFactoryOptions _options;

        public LazyPipeline(IServiceProvider serviceProvider, IEnumerable<Type> middlewareTypes, string clientName)
        {
            _serviceProvider = serviceProvider;
            _middlewareTypes = middlewareTypes.ToArray();
            _options = ((IOptionsSnapshot<AnshanFactoryOptions>)
                _serviceProvider.GetService(typeof(IOptionsSnapshot<AnshanFactoryOptions>)))?.Get(clientName);
        }

        public Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken)
        {
            IPipelineContext context = new PipelineContext();

            var index = 0;

            var middlewares = _middlewareTypes.Where(middlewareType =>
                    _options.Types.Contains(middlewareType))
                .ToArray();


            Task<TResponse> Next()
            {
                if (middlewares.Length == index)
                {
                    return Task.FromResult<TResponse>(default);
                }

                var middlewareType = middlewares[index++];

                if (_serviceProvider.GetService(middlewareType) is not IMiddleware<TRequest, TResponse> middleware)
                {
                    throw new MiddlewareNotResolvedException(middlewareType);
                }

                return middleware.RunAsync(request, context, Next, cancellationToken);
            }

            return Next();
        }
    }
}