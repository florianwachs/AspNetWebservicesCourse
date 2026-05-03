namespace TechConf.Api.Infrastructure.Exceptions;

public sealed class ConflictException(string message) : Exception(message);
