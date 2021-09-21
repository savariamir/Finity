using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Request;
using Finity.Shared;

namespace Finity.Authentication
{
    public class AuthenticationMiddleware : IMiddleware<AnshanHttpRequestMessage, HttpResponseMessage>
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthenticationMiddleware(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage> RunAsync(
            AnshanHttpRequestMessage request, 
            IPipelineContext context,
            Func<Type,Task<HttpResponseMessage>> next,
            Action<MetricValue> setMetric,
            CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetToken(request.Name, cancellationToken);
            request.HttpRequest.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            return await next(MiddlewareType);
        }
        
        public Type MiddlewareType { get; set; } 
            = typeof(AuthenticationMiddleware);
    }
}