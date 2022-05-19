using EfCoreCqrs.Features;

namespace EfCoreCqrs.Api;

public static class ApiResultExtensions
{
    public static IResult ToIResult<TData>(this ApiResult<TData> s)
    {
        if (s.Success)
        {
            return Results.Ok(s.Data);
        }

        return Results.BadRequest(s.Error);
    }
}
