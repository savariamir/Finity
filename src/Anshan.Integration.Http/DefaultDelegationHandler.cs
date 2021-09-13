using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyPipe;

namespace Anshan.Integration.Http
{
    internal class DefaultDelegationHandler : DelegatingHandler
    {
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;
        private readonly string _clientName;

        public DefaultDelegationHandler(string clientName, IServiceProvider services)
        {
            _clientName = clientName;
            _pipeline = (IPipeline<AnshanHttpRequestMessage, HttpResponseMessage>)services.GetService(
                typeof(IPipeline<AnshanHttpRequestMessage, HttpResponseMessage>));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await _pipeline.RunAsync(new AnshanHttpRequestMessage
            {
                Request = request,
                SendAsync = () => base.SendAsync(request, cancellationToken),
                ClientName = _clientName
            }, cancellationToken);

            return response;
        }
    }
}