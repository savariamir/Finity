using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EasyPipe;

namespace Anshan.Integration.Http
{
    internal class DefaultDelegationHandler : DelegatingHandler
    {
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;

        public DefaultDelegationHandler(IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> pipeline)
        {
            _pipeline = pipeline;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await _pipeline.RunAsync(new AnshanHttpRequestMessage
            {
                Request = request,
                SendAsync = () => base.SendAsync(request, cancellationToken)
            }, cancellationToken);

            return response;
        }
    }
}