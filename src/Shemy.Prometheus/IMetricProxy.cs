using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shemy.Prometheus
{
    public interface IMetricProxy
    {
        Task<HttpResponseMessage> ExecuteAsync(string name, Func<Task<HttpResponseMessage>> func);
    }
}