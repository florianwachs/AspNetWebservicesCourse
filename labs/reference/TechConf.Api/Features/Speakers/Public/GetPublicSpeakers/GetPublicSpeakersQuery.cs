using MediatR;
using TechConf.Api.Infrastructure.Pagination;

namespace TechConf.Api.Features.Speakers.Public.GetPublicSpeakers;

public sealed record GetPublicSpeakersQuery(
    string? Search,
    string? Company,
    string? Sort,
    int Page,
    int PageSize) : IRequest<PagedResponse<PublicSpeakerSummary>>;
