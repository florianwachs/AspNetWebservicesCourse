using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class SimpleController : ApiController
    {
        private static List<string> greetings = new List<string>() { "Hallo", "Servus", "Griaß di", "Habedere" };

        public string[] GetGreetings()
        {
            return greetings.ToArray();
        }

        public IHttpActionResult GetGreeting(int id)
        {
            if (id > 0 && id <= greetings.Count)
            {
                var greeting = greetings[id - 1];
                return Ok(greeting); // 200 OK
            }
            return NotFound(); // 404 Not Found
        }

        public IHttpActionResult GetGreetingContainingText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return NotFound();
            }
            var greeting = greetings.Where(g =>
                g.ToLower().Contains(text.ToLower()))
                .FirstOrDefault();

            return greeting == null ? (IHttpActionResult)NotFound() : Ok(greeting);
        }

        // WebAPI wandelt automatisch den JSON / XML Payload des Requests
        // in das POCO Model um
        public IHttpActionResult PostGreeting(GreetingDto greeting)
        {
            if (greeting == null || !ModelState.IsValid)
            {
                return BadRequest(GetErrorMessage() ?? "No greeting provided");
            }

            if (greetings.Any(g => g == greeting.Greeting))
            {
                return BadRequest("duplicate");
            }

            greetings.Add(greeting.Greeting);
            return Ok(greeting.Greeting);
        }

        // WebAPI versucht Simple Types aus der URL zu extrahieren
        // Es muss expizit angegeben werden das dieser Simple Type aus dem Body geholt werden soll
        // Es wird in jedem Fall empfohlen, Objekte für den Transport von Daten zu verwenden
        // und Simple Types nur für URL-Parameter
        // Gepostet werden muss mit form-data und =[value]
        //public IHttpActionResult Post([FromBody]string greeting)
        //{
        //    if (string.IsNullOrWhiteSpace(greeting))
        //    {
        //        return BadRequest("no greeting provided"); // 400 Bad Request
        //    }

        //    if (greetings.Any(g => g == greeting))
        //    {
        //        return BadRequest("duplicate");
        //    }

        //    greetings.Add(greeting);
        //    return Ok(greeting);
        //}

        public class GreetingDto
        {
            [Required]
            public string Greeting { get; set; }
        }

        private string GetErrorMessage()
        {
            if (ModelState.IsValid)
            {
                return null;
            }

            return string.Join(";", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }
    }
}
