using System.Net.Http.Json;

namespace ConsoleApp3.Clients;

public class StarwarsApiClient
{
    private readonly HttpClient _client;
    public StarwarsApiClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://swapi.dev/api/");
    }

    public async Task<Character> GetCharacterById(string id)
    {
        var response = await _client.GetAsync($"people/{id}");
        var character = await response.Content.ReadFromJsonAsync<Character>();
        return character;
    }
    
}