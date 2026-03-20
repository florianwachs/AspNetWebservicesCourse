namespace TechConf.Api.Models;

public class Speaker
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Company { get; set; }
    public List<Session> Sessions { get; set; } = [];
}
