namespace ConsoleApp3;

public class Character
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("height")]
    public string Height { get; set; }

    [JsonPropertyName("mass")]
    public string Mass { get; set; }

    [JsonPropertyName("hair_color")]
    public string HairColor { get; set; }

    [JsonPropertyName("skin_color")]
    public string SkinColor { get; set; }

    [JsonPropertyName("eye_color")]
    public string EyeColor { get; set; }

    [JsonPropertyName("birth_year")]
    public string BirthYear { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; }

    [JsonPropertyName("homeworld")]
    public Uri Homeworld { get; set; }

    [JsonPropertyName("films")]
    public List<Uri> Films { get; set; }

    [JsonPropertyName("species")]
    public List<Uri> Species { get; set; }

    [JsonPropertyName("vehicles")]
    public List<Uri> Vehicles { get; set; }

    [JsonPropertyName("starships")]
    public List<Uri> Starships { get; set; }

    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    [JsonPropertyName("edited")]
    public DateTimeOffset Edited { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }
}


