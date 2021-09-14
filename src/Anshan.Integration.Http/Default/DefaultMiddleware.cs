using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Request;
using Anshan.Integration.Pipeline.Abstractions;

namespace Anshan.Integration.Http.Default
{
    public class DefaultMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request,
            IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var response = await request.SendAsync();

            return response;
        }
    }
}