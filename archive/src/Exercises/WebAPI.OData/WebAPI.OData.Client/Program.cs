using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.OData.Client.Default;
using WebAPI.OData.Client.WebAPI.OData.Models;

namespace WebAPI.OData.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceUri = "http://localhost:7403/odata";
            var container = new Default.Container(new Uri(serviceUri));

            ListAllBooks(container);
            FilterBooks(container);
        }

        private static void ListAllBooks(Container container)
        {
            var books = container.Books;
            PrintBooks(books);
        }

        private static void FilterBooks(Container container)
        {
            PrintBooks(container.Books.Expand(b=>b.Authors).Where(b => b.Title.Contains("C#") && b.Authors.Any(a=>a.FirstName.StartsWith("Chuck"))));
        }

        private static void PrintBooks(IQueryable<Book> books)
        {
            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} {book.Price}");
            }
        }
    }
}
