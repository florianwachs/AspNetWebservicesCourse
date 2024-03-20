namespace EfCoreCqrs.Domain;

public enum ContactInfoTypes
{
    Email,
    Phone,
    Postal,
}

public class ContactInfo
{
    public int Id { get; set; }
    public int Description { get; set; }
    public ContactInfoTypes Type { get; set; }
}
