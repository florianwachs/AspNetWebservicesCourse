using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AspNetCoreApiVersioning.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // API Version kann über eine Extension Method am Context abgerufen werden.
            var apiVersion = HttpContext.GetRequestedApiVersion();

            return new string[] { apiVersion.ToString() };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id, ApiVersion apiVersion)
        {
            // API Version kann über den Modelbinder direkt injected werden.
            return apiVersion.ToString();
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
