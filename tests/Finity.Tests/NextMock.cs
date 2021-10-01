using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finity.Tests.Cache
{
    public class NextMock
    {
        public int CallsCount = 0;
        public HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

        public Task<HttpResponseMessage> Next(Type arg)
        {
            CallsCount++;
            return Task.FromResult(Response);
        }
    }
}