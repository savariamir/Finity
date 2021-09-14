using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.Request
{
    public class AnshanHttpRequestMessage
    {
        public Func<Task<HttpResponseMessage>> SendAsync { set; get; }
        public HttpRequestMessage HttpRequestMessage { set; get; }  
        public string ClientName { set; get; }
    }
}