using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Anshan.Integration.Http
{
    public class AnshanHttpRequestMessage
    {
        public Func<Task<HttpResponseMessage>> SendAsync { set; get; }
        public HttpRequestMessage Request { set; get; }
    }
}