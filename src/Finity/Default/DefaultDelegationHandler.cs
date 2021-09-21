using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Finity.Pipeline.Abstractions;
using Finity.Request;

namespace Finity.Default
{
    public class DefaultDelegationHandler : DelegatingHandler
    {
        private readonly IPipeline<FinityHttpRequestMessage, HttpResponseMessage> _pipeline;
        private readonly string _clientName;

        public DefaultDelegationHandler(
            string clientName,
            IPipeline<FinityHttpRequestMessage, HttpResponseMessage> serviceProvider)
        {
            _clientName = clientName;
            _pipeline = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await _pipeline.RunAsync(new FinityHttpRequestMessage
            {
                HttpRequest = request,
                SendAsync = () => base.SendAsync(request, cancellationToken),
                Name = _clientName
            }, cancellationToken);

            return response;
        }
    }
}