using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anshan.Integration.Http.Http
{
    public class AnshanHttp : IAnshanHttp
    {
        private readonly HttpClient _httpClient;

        public AnshanHttp(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("test");
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
            
            if (!response.IsSuccessStatusCode) throw new Exception();
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }
    }
}