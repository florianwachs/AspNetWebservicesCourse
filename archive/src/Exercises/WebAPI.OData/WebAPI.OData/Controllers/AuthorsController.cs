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
    public class AuthorsController : ODataController
    {
        BookDbContext db = new BookDbContext();

        // Get: /odata/Authors
        [EnableQuery]
        [ODataRoute("Authors")]
        [HttpGet]
        public IQueryable<Author> QueryAll()
        {
            return db.Authors;
        }

        // Get: /odata/Authors({key})
        [EnableQuery]
        [HttpGet]
        [ODataRoute("Authors({key})")]
        public SingleResult<Author> QueryAuthorById([FromODataUri] int key)
        {
            IQueryable<Author> result = db.Authors.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        // Get: /odata/Authors({key})/Books
        [EnableQuery]
        [HttpGet]
        [ODataRoute("Authors({key})/Books")]
        public IQueryable<Book> QueryRelatedBooks([FromODataUri] int key)
        {
            return db.Authors.Where(p => p.Id == key).SelectMany(b => b.Books);
        }

        // Mit Convention Based Routing
        //// Get: /odata/Authors({key})/Books
        //[EnableQuery]
        //public IQueryable<Book> GetBooks([FromODataUri] int key)
        //{
        //    return db.Authors.Where(p => p.Id == key).SelectMany(b => b.Books);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}