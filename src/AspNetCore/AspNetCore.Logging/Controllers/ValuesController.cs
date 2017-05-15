using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Logging.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ILogger Logger { get; set; }

        // Logger manuell über Factory erzeugen
        //public ValuesController(ILoggerFactory logger)
        //{
        //    Logger = logger.CreateLogger<ValuesController>();
        //}

        // Logger vom DI-System erzeugen lassen
        public ValuesController(ILogger<ValuesController> logger)
        {
            Logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
