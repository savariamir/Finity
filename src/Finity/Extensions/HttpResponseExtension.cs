using System.Net;
using System.Net.Http;

namespace Finity.Extensions
{
    public static class HttpResponseExtension
    {
        public static bool IsSuccessful(this HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ||
                   response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest;
        }
    }
}