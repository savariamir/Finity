using System.Threading;
using System.Threading.Tasks;

namespace Finity.Authentication
{
    public interface ITokenProvider
    {
        Task<string> GetToken(string name, CancellationToken cancellationToken);
    }
}