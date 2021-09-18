using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finity.Pipeline.Abstractions
{
    public interface IMiddleware<in TRequest, TResponse>
    {
        Task<TResponse> RunAsync(TRequest request,
                                 IPipelineContext context,
                                 Func<Task<TResponse>> next,
                                 CancellationToken cancellationToken);
    }
}