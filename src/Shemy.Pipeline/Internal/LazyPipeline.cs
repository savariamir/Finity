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
        private readonly string _clientName;

        public LazyPipeline(IServiceProvider serviceProvider, IEnumerable<Type> middlewareTypes, string clientName)
        {
            _serviceProvider = serviceProvider;
            _clientName = clientName;
            _middlewareTypes = middlewareTypes.ToArray();
            _options = ((IOptionsSnapshot<AnshanFactoryOptions>)
                _serviceProvider.GetService(typeof(IOptionsSnapshot<AnshanFactoryOptions>)))?.Get(clientName);
        }

        public Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken)
        {
            Func<Task<TResponse>> middlewareDelegate = null;
            IPipelineContext context = new PipelineContext();

            int index = 0;

            var middlewares = new List<Type>();
            foreach (var middlewareType in _middlewareTypes)
            {
                if (_options.Types.Contains(middlewareType))
                    middlewares.Add(middlewareType);
            }


            middlewareDelegate = () =>
            {
                if (middlewares.ToArray().Length == index)
                {
                    return Task.FromResult<TResponse>(default);
                }

                var middlewareType = middlewares.ToArray()[index++];


                var middleware = _serviceProvider.GetService(middlewareType) as IMiddleware<TRequest, TResponse>;

                if (middleware is null)
                {
                    throw new MiddlewareNotResolvedException(middlewareType);
                }

                return middleware.RunAsync(request, context, middlewareDelegate, cancellationToken);
            };

            return middlewareDelegate();
        }
    }
}