using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Finity.Authentication.Abstractions;
using Finity.Pipeline.Abstractions;
using Finity.Request;

namespace Finity.Authentication.Internals
{
    public class AuthenticationMiddleware : IMiddleware<FinityHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthenticationMiddleware(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(FinityHttpRequestMessage request,
            IPipelineContext context,
            Func<Type, Task<HttpResponseMessage>> next,
            CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetTokenAsync(request.Name, cancellationToken);
            request.HttpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await next(Type);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            token = await _tokenProvider.GetNewTokenAsync(request.Name, cancellationToken);
            request.HttpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = await next(Type);
            
            return response;
        }

        public Type Type { get; set; }
            = typeof(AuthenticationMiddleware);
    }
}