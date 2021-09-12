using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyPipe;

namespace Anshan.Integration.Http
{
    internal class HttpEngine : IHttpEngine
    {
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;
        public HttpEngine(IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> pipeline)
        {
            _pipeline = pipeline;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                         CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> sendAsync)
        {
            return await _pipeline.RunAsync(new AnshanHttpRequestMessage
             {
                 Request = request,
                 SendAsync = sendAsync
             }, cancellationToken);

        }
    }
}