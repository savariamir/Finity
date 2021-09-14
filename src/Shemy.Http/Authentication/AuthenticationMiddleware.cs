using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Http.Request;
using Shemy.Pipeline.Abstractions;

namespace Shemy.Http.Authentication
{
    public class AuthenticationMiddleware: IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        public Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context, Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}