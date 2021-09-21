using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Finity.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {

        private readonly HttpClient _httpClient;

        public SampleController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("finity");
        }
        

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://run.mocky.io/v3/10cb934a-b8be-4b75-8b2f-aef09574bd7e");
            var response = await _httpClient.SendAsync(request);

            
            if (!response.IsSuccessStatusCode) throw new Exception();


            var responseStream =await  response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<object>(responseStream);


            return Ok(data);
        }
    }
}