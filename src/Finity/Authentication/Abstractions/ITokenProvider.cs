using System.Threading;
using System.Threading.Tasks;

namespace Finity.Authentication.Abstractions
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync(string name, CancellationToken cancellationToken);
        Task<string> GetNewTokenAsync(string name, CancellationToken cancellationToken);
    }
}