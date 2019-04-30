using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.BooksClient
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content) where T : class
        {
            var rawJson = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            return JsonConvert.DeserializeObject<T>(rawJson);
        }
    }
}