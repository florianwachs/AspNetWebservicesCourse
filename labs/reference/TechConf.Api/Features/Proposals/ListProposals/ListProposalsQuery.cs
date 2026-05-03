using MediatR;
using TechConf.Api.Infrastructure.Pagination;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals.ListProposals;

public sealed record ListProposalsQuery(
    string? Search,
    ProposalStatus? Status,
    string? Sort,
    int Page,
    int PageSize) : IRequest<PagedResponse<ProposalListItemResponse>>;
