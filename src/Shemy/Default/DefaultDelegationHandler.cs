using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shemy.Pipeline.Abstractions;
using Shemy.Request;

namespace Shemy.Default
{
    internal class DefaultDelegationHandler : DelegatingHandler
    {
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;
        private readonly string _clientName;

        public DefaultDelegationHandler(string clientName,
            IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> serviceProvider)
        {
            _clientName = clientName;
            _pipeline = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await _pipeline.RunAsync(new AnshanHttpRequestMessage
            {
                HttpRequestMessage = request,
                SendAsync = () => base.SendAsync(request, cancellationToken),
                ClientName = _clientName
            }, cancellationToken);

            return response;
        }
    }
}