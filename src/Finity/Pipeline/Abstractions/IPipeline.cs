using System.Threading;
using System.Threading.Tasks;

namespace Finity.Pipeline.Abstractions
{
    public interface IPipeline<in TRequest, TResponse>
    {
        Task<TResponse> RunAsync(TRequest request,
                                 CancellationToken cancellationToken = default);
    }
    
    public interface IPipeline<TResponse>
    {
        Task<TResponse> RunAsync(CancellationToken cancellationToken = default);
    }
}