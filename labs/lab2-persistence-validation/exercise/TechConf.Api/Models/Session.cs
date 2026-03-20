namespace TechConf.Api.Models;

public class Session
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Abstract { get; set; }
    public TimeSpan Duration { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public List<Speaker> Speakers { get; set; } = [];
}
