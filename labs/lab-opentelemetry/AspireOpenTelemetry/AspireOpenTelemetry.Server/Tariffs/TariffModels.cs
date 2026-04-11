namespace AspireOpenTelemetry.Server.Tariffs;

public sealed record GenerateTariffsRequest(int Count = 8, string? Scenario = null);

public sealed record TariffAuditRequest(string? FocusCountryCode = null);

public sealed record TradeDisputeRequest(string? CountryCode = null, string? Complaint = null);

public sealed record TariffBatchResponse(
    Guid BatchId,
    string Scenario,
    string Headline,
    DateTimeOffset GeneratedAt,
    int AbsurdityIndex,
    double AverageTariff,
    TariffAuditSummary? LatestAudit,
    TradeDisputeSummary? LatestDispute,
    IReadOnlyList<TariffCard> Tariffs);

public sealed record TariffAuditSummary(
    Guid AuditId,
    DateTimeOffset CreatedAt,
    string? FocusCountryCode,
    string? FocusCountryName,
    int FlaggedCountries,
    string Verdict,
    string DashboardHint);

public sealed record TradeDisputeSummary(
    Guid DisputeId,
    DateTimeOffset CreatedAt,
    string CountryCode,
    string CountryName,
    string Severity,
    string Outcome,
    bool HighlightAsError);

public sealed record TariffCard(
    string CountryCode,
    string CountryName,
    string ExportSpecialty,
    double TariffPercent,
    string TariffBand,
    int AbsurdityScore,
    string RiskLevel,
    string Volatility,
    string SillyReason,
    string MinisterNote);

public sealed record TariffInspectionResponse(
    Guid BatchId,
    TariffCard Tariff,
    string Story,
    string TraceTip,
    string LogHint,
    string MetricHint);

public sealed record TariffAuditResponse(
    Guid AuditId,
    Guid BatchId,
    DateTimeOffset CreatedAt,
    string Scenario,
    string? FocusCountryCode,
    string? FocusCountryName,
    int FlaggedCountries,
    double AverageTariff,
    TariffCard HighestTariff,
    string Verdict,
    IReadOnlyList<string> Findings,
    string DashboardHint);

public sealed record TradeDisputeResponse(
    Guid DisputeId,
    Guid BatchId,
    DateTimeOffset CreatedAt,
    string CountryCode,
    string CountryName,
    double TariffPercent,
    string Complaint,
    string Outcome,
    string Severity,
    string Ruling,
    bool HighlightAsError,
    string DashboardHint);

internal sealed record CountryProfile(
    string CountryCode,
    string CountryName,
    string ExportSpecialty,
    string BureaucraticTrigger,
    int AbsurdityBias);
