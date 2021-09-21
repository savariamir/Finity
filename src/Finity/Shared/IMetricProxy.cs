using System;
using System.Net.Http;
using System.Threading.Tasks;
using Finity.Request;

namespace Finity.Shared
{
    public interface IMetricProxy
    {
        Task<HttpResponseMessage> ExecuteAsync(FinityHttpRequestMessage request);
    }
}