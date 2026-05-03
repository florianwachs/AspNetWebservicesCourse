namespace TechConf.Api.Infrastructure.Exceptions;

public sealed class ForbiddenException(string message) : Exception(message);
