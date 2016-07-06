using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using WebAPI.OData.DataAccess;
using WebAPI.OData.Models;

namespace WebAPI.OData.Controllers
{
    public class AuthorsController : ODataController
    {
        BookDbContext db = new BookDbContext();

        [EnableQuery]
        public IQueryable<Author> Get()
        {
            return db.Authors;
        }

        [EnableQuery]
        public SingleResult<Author> Get([FromODataUri] int key)
        {
            IQueryable<Author> result = db.Authors.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}