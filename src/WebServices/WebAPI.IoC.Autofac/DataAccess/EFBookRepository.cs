using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.IoC.Autofac.Models;

namespace WebAPI.IoC.Autofac.DataAccess
{
    public class EFBookRepository : IBookRepository
    {
        private BookDbContext Context { get; set; }
        public EFBookRepository(BookDbContext context)
        {
            Context = context;
        }
        public Book Add(Book p)
        {
            Context.Books.Add(p);
            Context.SaveChanges();
            return p;
        }

        public bool Delete(int id)
        {
            var book = Context.Books.Where(p => p.Id == id).FirstOrDefault();
            if (book == null)
                return false;
            Context.Books.Remove(book);
            Context.SaveChanges();
            return true;
        }

        public IEnumerable<Book> GetAll()
        {
            return Context.Books.ToArray();
        }

        public Book GetById(int id)
        {
            return Context.Books.Where(p => p.Id == id).FirstOrDefault();
        }

        public Book Update(int id, Book update)
        {
            var book = Context.Books.Where(p => p.Id == id).FirstOrDefault();
            if (book == null)
                return null;

            book.Price = update.Price;
            book.Title = update.Title;
            book.ReleaseDate = update.ReleaseDate;
            book.Authors = update.Authors;

            Context.SaveChanges();
            return book;
        }
    }
}