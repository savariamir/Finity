using System.Net;
using System.Net.Http;

namespace Shemy.Extensions
{
    public static class HttpResponseExtension
    {
        public static bool IsSucceed(this HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ||
                   response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest;
        }
    }
}