using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Authentication
{
    internal class AuthenticationMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthenticationMiddleware(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage> RunAsync(AnshanHttpRequestMessage request, IPipelineContext context,
            Func<Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetToken(request.ClientName);
            request.HttpRequestMessage.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            return await next();
        }
    }
}