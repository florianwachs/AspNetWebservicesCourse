namespace TechConf.Api.Exceptions;

public class NotFoundException(string entityName, object id)
    : Exception($"{entityName} with ID '{id}' was not found");
