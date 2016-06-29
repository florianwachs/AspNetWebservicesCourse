using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetSignalR.Controllers
{
    public class BookSpaController : Controller
    {
        // GET: BookSpa
        public ActionResult Index()
        {
            return View();
        }
    }
}