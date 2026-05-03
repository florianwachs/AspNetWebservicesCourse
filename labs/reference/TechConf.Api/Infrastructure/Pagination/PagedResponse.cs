namespace TechConf.Api.Infrastructure.Pagination;

public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Data,
    PaginationMetadata Pagination);
