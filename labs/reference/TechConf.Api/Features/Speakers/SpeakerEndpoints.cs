using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Features.Speakers.Me.DeleteMySpeakerProfile;
using TechConf.Api.Features.Speakers.Me.GetMySpeakerProfile;
using TechConf.Api.Features.Speakers.Me.UpsertMySpeakerProfile;
using TechConf.Api.Features.Speakers.Public.GetPublicSpeakerById;
using TechConf.Api.Features.Speakers.Public.GetPublicSpeakers;

namespace TechConf.Api.Features.Speakers;

public static class SpeakerEndpoints
{
    public static void MapSpeakerEndpoints(this IEndpointRouteBuilder app)
    {
        var speakers = app.NewVersionedApi("Speakers")
            .MapGroup("/api/v{version:apiVersion}/speakers")
            .HasDeprecatedApiVersion(1.0)
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .WithTags("Speakers")
            .RequireRateLimiting("public-api");

        speakers.MapGet("/", MapGetSpeakersV1)
            .WithSummary("List speakers (V1)")
            .Produces(StatusCodes.Status200OK)
            .MapToApiVersion(1.0);

        speakers.MapGet("/", MapGetSpeakersV2)
            .WithSummary("List speakers (V2)")
            .Produces(StatusCodes.Status200OK)
            .MapToApiVersion(2.0);

        speakers.MapGet("/{id:int}", MapGetSpeakerByIdV2)
            .WithSummary("Get speaker details (V2)")
            .Produces<SpeakerDetailV2>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status304NotModified)
            .MapToApiVersion(2.0);

        var me = app.MapGroup("/api/speaker-profile")
            .WithTags("Speaker Profile")
            .RequireAuthorization(PolicyNames.SpeakerProfileWrite);

        me.MapGet("/me", async (ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new GetMySpeakerProfileQuery(), ct)))
            .WithSummary("Get my speaker profile");

        me.MapPut("/me", async (UpsertMySpeakerProfileCommand command, ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(command, ct)))
            .WithSummary("Create or update my speaker profile");

        me.MapDelete("/me", async Task<IResult> (ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteMySpeakerProfileCommand(), ct);
            return TypedResults.NoContent();
        }).WithSummary("Delete my speaker profile");
    }

    private static async Task<IResult> MapGetSpeakersV1(
        HttpContext httpContext,
        [AsParameters] PublicSpeakerQueryParameters parameters,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(parameters.ToQuery(), cancellationToken);
        WritePaginationLinks(httpContext, "1", parameters, response.Pagination);

        return TypedResults.Ok(new
        {
            data = response.Data.Select(x => new SpeakerSummaryV1(x.Id, x.Name)).ToArray(),
            pagination = response.Pagination
        });
    }

    private static async Task<IResult> MapGetSpeakersV2(
        HttpContext httpContext,
        [AsParameters] PublicSpeakerQueryParameters parameters,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(parameters.ToQuery(), cancellationToken);
        WritePaginationLinks(httpContext, "2", parameters, response.Pagination);

        return TypedResults.Ok(new
        {
            data = response.Data.Select(x => new SpeakerSummaryV2(
                x.Id,
                x.Name,
                x.Tagline,
                x.Company,
                x.City,
                x.AcceptedTalkCount,
                x.RecentTalks)).ToArray(),
            pagination = response.Pagination
        });
    }

    private static async Task<IResult> MapGetSpeakerByIdV2(
        int id,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetPublicSpeakerByIdQuery(id), cancellationToken);
        var etag = CreateWeakEtag(response.Id, response.UpdatedAtUtc);

        if (httpContext.Request.Headers.IfNoneMatch.Any(value => string.Equals(value, etag, StringComparison.Ordinal)))
        {
            return TypedResults.StatusCode(StatusCodes.Status304NotModified);
        }

        httpContext.Response.Headers.ETag = etag;
        httpContext.Response.Headers.LastModified = response.UpdatedAtUtc.ToUniversalTime().ToString("R");
        httpContext.Response.Headers.CacheControl = "public, max-age=60";

        return TypedResults.Ok(response);
    }

    private static void WritePaginationLinks(
        HttpContext httpContext,
        string version,
        PublicSpeakerQueryParameters parameters,
        Infrastructure.Pagination.PaginationMetadata pagination)
    {
        var links = new List<string>();
        if (pagination.Page < pagination.TotalPages)
        {
            links.Add($"<{BuildUrl(version, parameters, pagination.Page + 1)}>; rel=\"next\"");
        }

        if (pagination.Page > 1 && pagination.TotalPages > 0)
        {
            links.Add($"<{BuildUrl(version, parameters, pagination.Page - 1)}>; rel=\"prev\"");
        }

        if (links.Count > 0)
        {
            httpContext.Response.Headers.Link = string.Join(", ", links);
        }
    }

    private static string BuildUrl(string version, PublicSpeakerQueryParameters parameters, int page)
    {
        var queryValues = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = parameters.PageSize.ToString(),
            ["q"] = parameters.Q,
            ["company"] = parameters.Company,
            ["sort"] = parameters.Sort
        };

        return QueryHelpers.AddQueryString($"/api/v{version}/speakers", queryValues!);
    }

    private static string CreateWeakEtag(int speakerId, DateTimeOffset updatedAtUtc) =>
        $"W/\"speaker-{speakerId}-{updatedAtUtc.ToUnixTimeSeconds()}\"";
}

public sealed class PublicSpeakerQueryParameters
{
    public string? Q { get; init; }
    public string? Company { get; init; }
    public string? Sort { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }

    public GetPublicSpeakersQuery ToQuery() =>
        new(
            Q,
            Company,
            Sort,
            Page is > 0 ? Page.Value : 1,
            Math.Clamp(PageSize ?? 6, 1, 25));
}
