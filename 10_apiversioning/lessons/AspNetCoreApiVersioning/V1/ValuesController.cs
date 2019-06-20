using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreApiVersioning.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // API Version kann über eine Extension Method am Context abgerufen werden.
            var apiVersion = HttpContext.GetRequestedApiVersion();

            return new [] {apiVersion.ToString()};
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id, ApiVersion apiVersion)
        {
            // API Version kann über den Modelbinder direkt injected werden.
            return apiVersion.ToString();
        }
    }
}