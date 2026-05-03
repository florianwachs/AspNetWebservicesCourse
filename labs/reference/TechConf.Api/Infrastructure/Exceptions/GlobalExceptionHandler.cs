using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TechConf.Api.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblem(httpContext, validationException),
            NotFoundException => CreateProblem(httpContext, StatusCodes.Status404NotFound, "Not Found", exception.Message, "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.5"),
            ConflictException => CreateProblem(httpContext, StatusCodes.Status409Conflict, "Conflict", exception.Message, "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.10"),
            ForbiddenException => CreateProblem(httpContext, StatusCodes.Status403Forbidden, "Forbidden", exception.Message, "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.4"),
            _ => CreateProblem(httpContext, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.", "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1")
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblem(
        HttpContext httpContext,
        int status,
        string title,
        string detail,
        string type) =>
        new()
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = httpContext.Request.Path
        };

    private static ProblemDetails CreateValidationProblem(HttpContext httpContext, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => ToCamelCase(error.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

        var problemDetails = CreateProblem(
            httpContext,
            StatusCodes.Status422UnprocessableEntity,
            "Validation failed",
            "One or more validation errors occurred.",
            "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.21");

        problemDetails.Extensions["errors"] = errors;
        return problemDetails;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "request";
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
