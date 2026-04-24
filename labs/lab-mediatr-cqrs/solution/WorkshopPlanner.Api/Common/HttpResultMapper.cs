using WorkshopPlanner.Api.Contracts;

namespace WorkshopPlanner.Api.Common;

public static class HttpResultMapper
{
    public static IResult FromResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
            return onSuccess(result.Value);

        return result.Error!.Type switch
        {
            ErrorType.Validation => Results.BadRequest(new ErrorResponse(result.Error.Message)),
            ErrorType.Conflict => Results.Conflict(new ErrorResponse(result.Error.Message)),
            ErrorType.NotFound => Results.NotFound(new ErrorResponse(result.Error.Message)),
            _ => Results.BadRequest(new ErrorResponse(result.Error.Message))
        };
    }
}
