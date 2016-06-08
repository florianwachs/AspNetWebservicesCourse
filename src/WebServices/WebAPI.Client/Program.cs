using WebAPI.Client.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64000");

                // Standardmäßig JSON anfordern
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var books = await GetBooksAsync(client);

                var newBook = new Book(0, "ISBN982828", "My New Book", 0);
                var created = await CreateBook(client, newBook);

                created.Price = 100;
                var updated = await UpdateBook(client, created.Id, created);

                books = await GetBooksAsync(client);
            }
        }

        private static async Task<Book[]> GetBooksAsync(HttpClient client)
        {
            var response = await client.GetAsync("/api/books");
            response.EnsureSuccessStatusCode();

            var books = await response.Content.ReadAsAsync<Book[]>();
            return books;
        }

        private static async Task<Book> CreateBook(HttpClient client, Book book)
        {
            var response = await client.PostAsJsonAsync("/api/books", book);
            response.EnsureSuccessStatusCode();

            var newBook = await response.Content.ReadAsAsync<Book>();
            return newBook;
        }

        private static async Task<Book> UpdateBook(HttpClient client, int id, Book book)
        {
            var response = await client.PutAsJsonAsync("/api/books/" + id, book);
            response.EnsureSuccessStatusCode();

            var updated = await response.Content.ReadAsAsync<Book>();
            return updated;
        }
    }
}
