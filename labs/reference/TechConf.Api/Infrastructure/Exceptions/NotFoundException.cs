namespace TechConf.Api.Infrastructure.Exceptions;

public sealed class NotFoundException(string resourceName, object id)
    : Exception($"{resourceName} with id '{id}' was not found.");
