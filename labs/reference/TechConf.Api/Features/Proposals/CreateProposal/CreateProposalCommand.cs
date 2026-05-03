using MediatR;

namespace TechConf.Api.Features.Proposals.CreateProposal;

public sealed record CreateProposalCommand(
    int EventId,
    string Title,
    string Abstract,
    int DurationMinutes,
    string Track) : IRequest<ProposalDetailResponse>;
