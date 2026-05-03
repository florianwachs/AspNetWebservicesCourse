using MediatR;

namespace TechConf.Api.Features.Proposals.GetProposalById;

public sealed record GetProposalByIdQuery(int ProposalId) : IRequest<ProposalDetailResponse>;
