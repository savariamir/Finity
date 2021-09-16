using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Default
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