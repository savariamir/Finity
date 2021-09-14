using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shemy.Http.CircuitBreaker
{
    internal interface ICircuitBreakerEngine 
    {
        Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> next);
    }
}