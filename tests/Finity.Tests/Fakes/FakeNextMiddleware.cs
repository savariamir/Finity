using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finity.Tests.Fakes
{
    public class FakeNextMiddleware
    {
        public int SuccessCallsCount = 0;
        public int FailureCallsCount = 0;
        private int _retriesCount = 0;
        public readonly HttpResponseMessage Response = new(HttpStatusCode.OK);
        public readonly HttpResponseMessage FailureResponse = new(HttpStatusCode.InternalServerError);

        public Task<HttpResponseMessage> SuccessNext(Type arg)
        {
            SuccessCallsCount++;
            return Task.FromResult(Response);
        }
        
        public Task<HttpResponseMessage> FailureNext(Type arg)
        {
            FailureCallsCount++;
            return Task.FromResult(FailureResponse);
        }
        
        public Task<HttpResponseMessage> SuccessNextAfterSomeRetries(Type arg)
        {
            if (FailureCallsCount > _retriesCount)
            {
                SuccessCallsCount++;
                return Task.FromResult(Response);
            }
            
            FailureCallsCount++;
            return Task.FromResult(FailureResponse);
        }

        public FakeNextMiddleware SuccessNextAfter(int retireCount)
        {
            _retriesCount = retireCount;
            return this;
        }
    }
}