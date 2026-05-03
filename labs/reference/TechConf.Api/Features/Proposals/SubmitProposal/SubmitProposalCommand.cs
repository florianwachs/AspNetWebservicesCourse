using MediatR;

namespace TechConf.Api.Features.Proposals.SubmitProposal;

public sealed record SubmitProposalCommand(int ProposalId) : IRequest<ProposalDetailResponse>;
