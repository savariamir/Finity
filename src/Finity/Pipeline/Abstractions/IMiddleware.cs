using System;
using System.Threading;
using System.Threading.Tasks;
using Finity.Shared;
using Finity.Shared.Metrics;

namespace Finity.Pipeline.Abstractions
{
    public interface IMiddleware<in TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request,
            IPipelineContext context,
            Func<Type,Task<TResponse>> next,
            CancellationToken cancellationToken);

        Type Type {  set; get; }
    }
}   