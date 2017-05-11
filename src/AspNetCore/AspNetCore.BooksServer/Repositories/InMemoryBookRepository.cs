using AspNetCore.BooksServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.BooksServer.Repositories
{
    public class InMemoryBookRepository : IBookRepository
    {
        private static int id = 4;

        // ACHTUNG: nur zu DEMO-Zwecken. Dictionary ist nicht threadsafe!
        private static readonly Dictionary<int, Book> books = new Dictionary<int, Book>()
        {
            {1,new Book(1,"1430242337", "C# Pro", 30, new []{"Troelson"}, DateTime.Now.AddYears(-2))},
            {2,new Book(2,"161729134X", "C# in Depth", 40, new []{"Skeet"}, DateTime.Now.AddMonths(-2))},
            {3,new Book(3,"1449320104", "C# in a Nutshell", 40, new []{"Albahari"}, DateTime.Now.AddMonths(-2))},
            {4,new Book(4,"0596807260", "Entity Framework 6", 20, new []{"Lerman"}, DateTime.Now.AddMonths(-2))},
        };

        public Book Add(Book p)
        {
            p.Id = GetUniqueId();
            books.Add(p.Id, p);
            return p;
        }

        public bool Delete(int id)
        {
            return books.Remove(id);
        }

        public IEnumerable<Book> GetAll()
        {
            return books.Values;
        }

        public Book GetById(int id)
        {
            Book p;
            return books.TryGetValue(id, out p) ? p : null;
        }

        public Book Update(int id, Book p)
        {
            if (!books.ContainsKey(id))
            {
                return null;
            }

            p.Id = id;
            books[id] = p;
            return p;
        }
        private int GetUniqueId()
        {
            return id++;
        }
    }
}
