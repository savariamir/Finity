using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finity.Prometheus
{
    public interface IMetricProxy
    {
        Task<HttpResponseMessage> ExecuteAsync(string name, Func<Task<HttpResponseMessage>> func);
    }
}