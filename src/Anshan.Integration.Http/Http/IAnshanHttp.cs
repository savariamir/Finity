using System.Threading.Tasks;

namespace Anshan.Integration.Http.Http
{
    public interface IAnshanHttp
    {
        Task<T> GetAsync<T>(string uri);
    }
}