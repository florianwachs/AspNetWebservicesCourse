using MediatR;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals.ReviewProposal;

public sealed record ReviewProposalCommand(
    int ProposalId,
    ProposalStatus TargetStatus,
    string? Note) : IRequest<ProposalDetailResponse>;
