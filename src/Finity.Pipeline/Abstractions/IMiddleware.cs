using System;
using System.Threading;
using System.Threading.Tasks;
using Finity.Core;

namespace Finity.Pipeline.Abstractions
{
    public interface IMiddleware<in TRequest, TResponse>
    {
        Task<TResponse> RunAsync(TRequest request,
                                 IPipelineContext context,
                                 Func<Task<TResponse>> next,
                                 Action<MetricValue> setMetric,
                                 CancellationToken cancellationToken);
    }
}