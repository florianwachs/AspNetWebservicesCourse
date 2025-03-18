using System.Text.Json;
using System.Net.Http.Json;
using Xunit;
using System.Text.Json.Nodes;

namespace Advanced;

public class AsyncAwait
{
    [Fact]
    public void UseApiBlocking()
    {
        // Der Aufruf von Result blockiert den aufrufenden Thread,
        // bis das Ergebnis vorliegt
        var token = GetAuthTokenForUserAsync("Hansi", "Müller").Result;
        var greeting = GetChucksWisdomAsync(token).Result;

        Console.WriteLine(greeting);
    }

    [Fact]
    public void UseApiNonBlocking()
    {
        // Die Methode wird sofort verlassen nachdem die Tasks verkettet sind
        // der Hauptthread ist verantwortlich, das Ergebnis abzuwarten

        var task = GetAuthTokenForUserAsync("Hansi", "Müller")
            .ContinueWith(authTask => GetChucksWisdomAsync(authTask.Result))
            .ContinueWith(greetingTask => Console.WriteLine(greetingTask.Unwrap().Result));
    }

    // async void geht auch, der Aufrufer kann dan aber nicht auf die
    // Fertigstellung warten
    [Fact]
    public async Task UseApiAsync()
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
        var api = "https://api.chucknorris.io/jokes/random";
        var client = new HttpClient();
        var response = await client.GetFromJsonAsync<ChuckNorrisResponse>(api);
        return response.Value;
    }

    // Dynamische Möglichkeit auf JSON zuzugreifen (nicht empfohlen)
    private static string GetJokeFromJSON(string json)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonObject>(json);
        var joke = jsonNode["value"]?["joke"]?.GetValue<string>() ?? string.Empty;
        return joke;
    }

    public class JokeResponse
    {
        public JokeValue Value { get; set; }
    }

    public class JokeValue
    {
        public string Joke { get; set; }
    }

    public class ChuckNorrisResponse
    {
        public string Value { get; set; }
    }
}
