using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Request;
using Anshan.Integration.Pipeline.Abstractions;

namespace Anshan.Integration.Http.Default
{
    internal class DefaultDelegationHandler : DelegatingHandler
    {
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;
        private readonly string _clientName;

        public DefaultDelegationHandler(string clientName, IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> pipeline)
        {
            _clientName = clientName;
            _pipeline = pipeline;
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