using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.Retry.Appstractions
{
    internal interface IRetryEngine
    {
        Task<HttpResponseMessage> Retry(
            Func<Task<HttpResponseMessage>> sendAsync,
            CancellationToken cancellationToken);
    }
}