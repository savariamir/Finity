using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shemy.Authentication
{
    public interface ITokenProvider
    {
        Task<string> GetToken(string name, CancellationToken cancellationToken);
    }
}