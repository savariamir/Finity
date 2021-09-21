using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finity.Request
{
    public class FinityHttpRequestMessage
    {
        public Func<Task<HttpResponseMessage>> SendAsync { set; get; }
        public HttpRequestMessage HttpRequest { set; get; }  
        public string Name { set; get; }
    }
}