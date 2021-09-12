using System.Threading.Tasks;
using Anshan.Integration.Http.Http;
using Microsoft.AspNetCore.Mvc;

namespace Anshan.Integration.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IAnshanHttp _anshanHttp;
        public SampleController(IAnshanHttp anshanHttp)
        {
            _anshanHttp = anshanHttp;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ok = await _anshanHttp.GetAsync<object>("https://run.mocky.io/v3/10cb934a-b8be-4b75-8b2f-aef09574bd7e");
            
            return Ok(ok);
        }
    }
}