using System;
using System.Net.Http;
using System.Threading.Tasks;
using Finity.Request;
using Finity.Shared.Metrics;

namespace Finity.CircuitBreaker.Abstractions
{
    public interface ICircuitBreakerEngine
    {
        Task<HttpResponseMessage> ExecuteAsync(
            FinityHttpRequestMessage request,
            Func<Type,Task<HttpResponseMessage>> next
            );
    }
}