using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models;
using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookCatalogService _bookCatalog;

        public HomeController(IBookCatalogService bookCatalog)
        {
            _bookCatalog = bookCatalog;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookCatalog.GetBooks();

            // TODO Hier die anderen Services ansprechen

            var bookCatalogVm = BuildBookCatalogVm(books);

            return View(bookCatalogVm);
        }

        private BookCatalog BuildBookCatalogVm(IEnumerable<Book> books)
        {
            var catalogItems = books.Select(b =>
            new BookCatalogItem
            {
                Id = b.Id,
                Isbn = b.Isbn,
                Title = b.Title,
                ReleaseDate = b.ReleaseDate,
                Price = b.Price,
                Rating = b.Rating,
            }).ToArray();

            return new BookCatalog { Books = catalogItems };
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
