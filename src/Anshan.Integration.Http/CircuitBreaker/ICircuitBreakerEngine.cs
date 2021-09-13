using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.CircuitBreaker
{
    internal interface ICircuitBreakerEngine 
    {
        Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> next);
    }
}