namespace TechConf.Api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with ID '{id}' was not found") { }
}
