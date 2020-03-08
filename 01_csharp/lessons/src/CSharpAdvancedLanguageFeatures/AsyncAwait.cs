using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    public static class AsyncAwait
    {
        public static void UseApiBlocking()
        {
            // Der Aufruf von Result blockiert den aufrufenden Thread,
            // bis das Ergebnis vorliegt
            var token = GetAuthTokenForUserAsync("Hansi", "Müller").Result;
            var greeting = GetChucksWisdomAsync(token).Result;

            Console.WriteLine(greeting);
        }

        public static void UseApiNonBlocking()
        {
            // Die Methode wird sofort verlassen nachdem die Tasks verkettet sind
            // der Hauptthread ist verantwortlich, das Ergebnis abzuwarten

            var task = GetAuthTokenForUserAsync("Hansi", "Müller")
                .ContinueWith(authTask => GetChucksWisdomAsync(authTask.Result))
                .ContinueWith(greetingTask => Console.WriteLine(greetingTask.Unwrap().Result));
        }

        // async void geht auch, der Aufrufer kann dan aber nicht auf die
        // Fertigstellung warten
        public static async Task UseApiAsync()
        {
            // Errorhandling ist mit await einfach...
            try
            {
                var token = await GetAuthTokenForUserAsync("Hansi", "Müller");
                var greeting = await GetChucksWisdomAsync(token);

                Console.WriteLine(greeting);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Task<string> GetAuthTokenForUserAsync(string user, string password)
        {
            return Task.Delay(2000)
                .ContinueWith(preTask => Task.FromResult("12345")).Unwrap();
        }

        public static async Task<string> GetChucksWisdomAsync(string authToken)
        {
            var api = "http://api.icndb.com/jokes/random";
            var client = new HttpClient();
            var rawJson = await client.GetStringAsync(api);
            return GetJokeFromJSON(rawJson);
        }

        private static string GetJokeFromJSON(string json)
        {
            var jsonObj = JsonSerializer.Deserialize<dynamic>(json);
            var joke = jsonObj["value"]["joke"].ToString();
            return joke;
        }
    }
}
