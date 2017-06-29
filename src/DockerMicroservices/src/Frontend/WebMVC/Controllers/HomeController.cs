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
        private readonly IPricingService _priceService;
        private readonly IRatingService _ratingService;

        public HomeController(IBookCatalogService bookCatalog, IPricingService priceService, IRatingService ratingService)
        {
            _bookCatalog = bookCatalog;
            _priceService = priceService;
            _ratingService = ratingService;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookCatalog.GetBooks();

            // TODO: Aus Performance gründen könnten diese zwei Requests parallel laufen
            var prices = await _priceService.GetPricesForBooks(books);
            var ratings = await _ratingService.GetRatingsForBooks(books);

            var bookCatalogVm = BuildBookCatalogVm(books, prices, ratings);

            return View(bookCatalogVm);
        }

        private BookCatalog BuildBookCatalogVm(IEnumerable<Book> books, PriceResponse prices, RatingResponse ratings)
        {
            var priceMap = prices.Prices.ToDictionary(p => p.BookId);
            var ratingMap = ratings.Ratings.ToDictionary(p => p.BookId);

            var catalogItems = books.Select(b =>
            new BookCatalogItem
            {
                Id = b.Id,
                Isbn = b.Isbn,
                Title = b.Title,
                ReleaseDate = b.ReleaseDate,
                Price = priceMap.TryGetValue(b.Id, out Price price) ? price.Amount : 0m,
                Rating = ratingMap.TryGetValue(b.Id, out BookRating rating) ? rating.AvgRating : 0
            }).ToArray();

            return new BookCatalog { Books = catalogItems };
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
