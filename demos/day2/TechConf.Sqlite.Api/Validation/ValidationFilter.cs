using FluentValidation;

namespace TechConf.Api.Validation;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
        {
            return await next(context);
        }

        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is null)
        {
            return await next(context);
        }

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}
