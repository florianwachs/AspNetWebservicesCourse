namespace TechConf.Sessions.Api.Models;

public record Session(
    int Id,
    string Title,
    string Speaker,
    string Track,
    DateTime StartsAt,
    int DurationMinutes,
    bool IsPublished);

public record CreateSessionRequest(
    string Title,
    string Speaker,
    string Track,
    DateTime StartsAt,
    int DurationMinutes,
    bool IsPublished);

public record UpdateSessionRequest(
    string Title,
    string Speaker,
    string Track,
    DateTime StartsAt,
    int DurationMinutes,
    bool IsPublished);

public record PatchSessionRequest(
    string? Title,
    string? Speaker,
    string? Track,
    DateTime? StartsAt,
    int? DurationMinutes,
    bool? IsPublished);
