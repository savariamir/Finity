using System.Net.Http;

namespace Anshan.Integration.Http.Caching
{
    internal interface ICacheEngine
    {
        CacheResult<HttpResponseMessage> GetFromCache(HttpRequestMessage request);
        
        void Set(HttpRequestMessage request,HttpResponseMessage responseMessage);
    }
}