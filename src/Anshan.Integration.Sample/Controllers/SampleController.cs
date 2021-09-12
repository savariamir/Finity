using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Anshan.Integration.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {

        private readonly HttpClient _httpClient;

        public SampleController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("test");
        }
        

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                "https://run.mocky.io/v3/10cb934a-b8be-4b75-8b2f-aef09574bd7e"));
            if (!response.IsSuccessStatusCode) throw new Exception();


            var responseStream = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<object>(responseStream);


            return Ok(data);
        }
    }
}