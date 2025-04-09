
using System.Net.Http.Json;
using ConsoleApp3.Clients;

var client = new HttpClient();
var starwarsApi = new StarwarsApiClient(client);


for (int i = 1; i < 10; i++)
{
    try
    {
        var character = await starwarsApi.GetCharacterById(i.ToString());
        Console.WriteLine(character.Name);

    }
    catch (Exception e)
    {
        // upps , should not happen
  
    }
}





Console.WriteLine("Hello, World!");


