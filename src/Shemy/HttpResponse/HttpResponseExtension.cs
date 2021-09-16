using System.Net;
using System.Net.Http;

namespace Shemy.HttpResponse
{
    public static class HttpResponseExtension
    {
        public static bool IsSucceed(this HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound ||
                   response.StatusCode == HttpStatusCode.BadRequest;
        }
    }
}