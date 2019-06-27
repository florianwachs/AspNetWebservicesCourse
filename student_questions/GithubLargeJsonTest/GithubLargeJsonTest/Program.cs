using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubLargeJsonTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# App");
            var response = await httpClient.GetAsync("https://api.github.com/search/repositories?q=Tour%20of%20Heroes:name");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<RootResult>();

            Console.WriteLine("Hello World!");
        }
    }
}
