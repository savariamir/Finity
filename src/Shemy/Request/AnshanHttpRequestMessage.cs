using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shemy.Request
{
    public class AnshanHttpRequestMessage
    {
        public Func<Task<HttpResponseMessage>> SendAsync { set; get; }
        public HttpRequestMessage HttpRequest { set; get; }  
        public string Name { set; get; }
    }
}