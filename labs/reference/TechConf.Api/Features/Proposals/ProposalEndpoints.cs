using MediatR;
using TechConf.Api.Features.Proposals.CreateProposal;
using TechConf.Api.Features.Proposals.GetProposalById;
using TechConf.Api.Features.Proposals.ListProposals;
using TechConf.Api.Features.Proposals.ReviewProposal;
using TechConf.Api.Features.Proposals.SubmitProposal;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals;

public static class ProposalEndpoints
{
    public static void MapProposalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/proposals")
            .WithTags("Proposals")
            .RequireAuthorization();

        group.MapPost("/", async (CreateProposalCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return TypedResults.Created($"/api/proposals/{result.Id}", result);
        })
        .RequireAuthorization(PolicyNames.SpeakerAccess)
        .WithSummary("Create a proposal draft");

        group.MapGet("/", async ([AsParameters] ProposalQueryParameters parameters, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(parameters.ToQuery(), ct);
            return TypedResults.Ok(result);
        })
        .WithSummary("List proposals for the current speaker or organizer");

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new GetProposalByIdQuery(id), ct)))
            .WithSummary("Get a single proposal");

        group.MapPost("/{id:int}/submit", async (int id, ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new SubmitProposalCommand(id), ct)))
            .RequireAuthorization(PolicyNames.SpeakerAccess)
            .WithSummary("Submit a proposal for review");

        group.MapPost("/{id:int}/accept", async (int id, ReviewProposalRequest request, ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new ReviewProposalCommand(id, ProposalStatus.Accepted, request.Note), ct)))
            .RequireAuthorization(PolicyNames.ProposalReview)
            .WithSummary("Accept a submitted proposal");

        group.MapPost("/{id:int}/reject", async (int id, ReviewProposalRequest request, ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new ReviewProposalCommand(id, ProposalStatus.Rejected, request.Note), ct)))
            .RequireAuthorization(PolicyNames.ProposalReview)
            .WithSummary("Reject a submitted proposal");
    }
}

public sealed class ProposalQueryParameters
{
    public string? Q { get; init; }
    public string? Status { get; init; }
    public string? Sort { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }

    public ListProposalsQuery ToQuery()
    {
        ProposalStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(Status) &&
            Enum.TryParse<ProposalStatus>(Status, ignoreCase: true, out var value))
        {
            parsedStatus = value;
        }

        return new ListProposalsQuery(
            Q,
            parsedStatus,
            Sort,
            Page is > 0 ? Page.Value : 1,
            Math.Clamp(PageSize ?? 10, 1, 25));
    }
}
