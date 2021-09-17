using System;
using System.Threading.Tasks;

namespace Shemy.Authentication
{
    internal interface ITokenProvider
    {
        Task<string> GetToken(string clientName);
    }
}