using AspNetMVC5.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetMVC5.Controllers
{
    public class HomeController : Controller
    {
        // GET /Home/Simple
        public string Simple()
        {
            return "Hello World";
        }

        // GET /Home/Index
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult BookForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BookForm(BookModel book)
        {
            if (!ModelState.IsValid)
            {
                return View("BookForm");
            }
            var viewModel = new BookCreatedModel(book);
            return View("BookCreated", viewModel);
        }

        // GET /Home/JsonObject?firstName=Jason&lastName=Bourne
        public ActionResult JsonObject(string firstName, string lastName)
        {
            var obj = new { FirstName = firstName ?? "Jane", LastName = lastName ?? "Doe" };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        // GET /Home/ChuckKnowsBest?wisdomCount=3
        public async Task<ActionResult> ChuckKnowsBest(int? wisdomCount)
        {
            var cnt = Math.Min(wisdomCount ?? 1, 10);
            if (cnt == 0)
            {
                cnt = 1;
            }

            var tasks = Enumerable.Range(0, cnt)
                .Select(_ => GetChucksWisdomAsync("")).ToArray();

            await Task.WhenAll(tasks);

            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                sb.AppendLine(task.Result);
            }

            return Content(sb.ToString());
        }

        // GET http://localhost:60386/Home/Query?Page=2&CountPerPage=30
        public ActionResult Query(QueryRequest req)
        {
            var obj = new { Page = req.Page, Result = Enumerable.Range(0, req.CountPerPage).ToArray() };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        private async Task<string> GetChucksWisdomAsync(string authToken)
        {
            var api = "http://api.icndb.com/jokes/random";
            var client = new HttpClient();
            var rawJson = await client.GetStringAsync(api);
            return GetJokeFromJSON(rawJson);
        }

        private string GetJokeFromJSON(string json)
        {
            var jsonObj = JObject.Parse(json);
            var joke = jsonObj["value"]["joke"].ToString();
            return joke;
        }
    }
}