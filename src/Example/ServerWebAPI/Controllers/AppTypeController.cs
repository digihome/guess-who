using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GuessWho.Library;

namespace ServerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppTypeController : ControllerBase
    {
        [HttpGet()]
        public IActionResult GetApplicationType()
        {
            var result = AppTypeDetector.Detect();
            return Ok($"Detected application type: {result.Display}");
        }
    }
}
