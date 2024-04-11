namespace EfCoreCqrs.Domain;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public ICollection<Author> Authors { get; set; }
    public int Rating { get; set; }
    public decimal? Price { get; set; }
}
