using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anshan.Integration.Http.Caching;
using Anshan.Integration.Http.Configuration;
using Anshan.Integration.Http.Retry.Appstractions;
using EasyPipe;
using Microsoft.Extensions.Options;

namespace Anshan.Integration.Http
{
    internal class HttpEngine : IHttpEngine
    {
        private readonly AnshanFactoryOptions _anshanFactory;
        private readonly ICacheEngine _cacheEngine;
        private readonly IRetryEngine _retryEngine;
        private readonly IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> _pipeline;
        public HttpEngine(IOptions<AnshanFactoryOptions> options, ICacheEngine cacheEngine, IRetryEngine retryEngine, IPipeline<AnshanHttpRequestMessage, HttpResponseMessage> pipeline)
        {
            _cacheEngine = cacheEngine;
            _retryEngine = retryEngine;
            _pipeline = pipeline;
            _anshanFactory = options.Value;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                         CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> sendAsync)
        {
             await _pipeline.RunAsync(new AnshanHttpRequestMessage
             {
                 Request = request,
                 SendAsync = sendAsync
             });
             
            try
            {
                if (_anshanFactory.IsCacheEnabled)
                {
                    var cacheModel = _cacheEngine.GetFromCache(request);

                    if (cacheModel.Hit)
                        return cacheModel.Data;
                }

                if (_anshanFactory.CircuitBreaker)
                {
                }

                HttpResponseMessage response;
                if (_anshanFactory.IsRetryEnabled)
                    response = await _retryEngine.Retry(sendAsync, cancellationToken);
                else
                    response = await sendAsync();

                _cacheEngine.Set(request, response);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}