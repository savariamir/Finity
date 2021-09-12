using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anshan.Integration.Http
{
    internal interface IHttpEngine
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                            CancellationToken cancellationToken,
                                            Func<Task<HttpResponseMessage>> sendAsync);
    }
}