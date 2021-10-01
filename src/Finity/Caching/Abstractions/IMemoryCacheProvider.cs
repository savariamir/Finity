using System.Net.Http;
using Finity.Request;

namespace Finity.Caching.Abstractions
{
    public interface IMemoryCacheProvider
    {
        void SetToCache(FinityHttpRequestMessage request, HttpResponseMessage response);
        CacheResult<HttpResponseMessage> GetFromCache(string requestUri);
    }
}