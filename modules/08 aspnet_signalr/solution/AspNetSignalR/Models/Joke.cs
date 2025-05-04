using System.Text.Json.Serialization;

namespace AspNetSignalR.Models;

public class Joke
{
    [JsonPropertyName("value")]
    public required string Value { get; set; }
    
}