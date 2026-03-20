using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TechConf.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // TODO: Task 6 - Implement exception handling
        // - Log the exception
        // - Map exception type to status code (NotFoundException -> 404, etc.)
        // - Write ProblemDetails response
        // - Return true to indicate the exception was handled
        return false;
    }
}
