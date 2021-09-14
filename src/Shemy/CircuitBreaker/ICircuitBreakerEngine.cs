using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shemy.CircuitBreaker
{
    internal interface ICircuitBreakerEngine 
    {
        Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> next);
    }
}