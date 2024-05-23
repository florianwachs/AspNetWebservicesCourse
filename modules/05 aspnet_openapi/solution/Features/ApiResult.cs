namespace EfCoreCqrs.Features;

public record ApiResult<TData>
{
    public bool Success { get; init; }
    public TData? Data { get; init; }
    public string? Error { get; init; }

    public static ApiResult<TData> Successful(TData data) => new() { Data = data, Success = true, Error = "" };
    public static ApiResult<TData> Failure(string error) => new() { Data = default, Success = false, Error = error };
}

