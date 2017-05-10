using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.SimpleControllerSample.Controllers
{
    [Route("api/[controller]")]
    public class SimpleController : Controller
    {
        [HttpGet]
        public string GetGreeting()
        {
            return "Hello World";
        }
    }
}
