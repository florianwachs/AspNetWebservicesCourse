namespace TechConf.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Session> Sessions { get; set; } = [];
}
