using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPI.OData.DataAccess;
using WebAPI.OData.Models;

namespace WebAPI.OData.Controllers
{
    public class BooksController : ODataController
    {
        BookDbContext db = new BookDbContext();

        // GET: /odata/Books
        [EnableQuery]
        public IQueryable<Book> Get()
        {
            return db.Books;
        }

        // GET: /odata/Books({id})
        [EnableQuery]
        public SingleResult<Book> Get([FromODataUri] int key)
        {
            IQueryable<Book> result = db.Books.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}