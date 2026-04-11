namespace AspireOpenTelemetry.Server.Tariffs;

public sealed class TariffBatchEntity
{
    public Guid Id { get; set; }

    public string Scenario { get; set; } = string.Empty;

    public string Headline { get; set; } = string.Empty;

    public DateTimeOffset GeneratedAt { get; set; }

    public int AbsurdityIndex { get; set; }

    public double AverageTariff { get; set; }

    public List<TariffQuoteEntity> Quotes { get; set; } = [];

    public List<AuditRunEntity> Audits { get; set; } = [];

    public List<TradeDisputeEntity> Disputes { get; set; } = [];
}

public sealed class TariffQuoteEntity
{
    public Guid Id { get; set; }

    public Guid BatchId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string ExportSpecialty { get; set; } = string.Empty;

    public double TariffPercent { get; set; }

    public string TariffBand { get; set; } = string.Empty;

    public int AbsurdityScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string Volatility { get; set; } = string.Empty;

    public string SillyReason { get; set; } = string.Empty;

    public string MinisterNote { get; set; } = string.Empty;

    public TariffBatchEntity Batch { get; set; } = null!;
}

public sealed class AuditRunEntity
{
    public Guid Id { get; set; }

    public Guid BatchId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? FocusCountryCode { get; set; }

    public string? FocusCountryName { get; set; }

    public int FlaggedCountries { get; set; }

    public double AverageTariff { get; set; }

    public string HighestCountryCode { get; set; } = string.Empty;

    public string HighestCountryName { get; set; } = string.Empty;

    public string HighestExportSpecialty { get; set; } = string.Empty;

    public double HighestTariffPercent { get; set; }

    public string HighestTariffBand { get; set; } = string.Empty;

    public int HighestAbsurdityScore { get; set; }

    public string HighestRiskLevel { get; set; } = string.Empty;

    public string HighestVolatility { get; set; } = string.Empty;

    public string HighestSillyReason { get; set; } = string.Empty;

    public string HighestMinisterNote { get; set; } = string.Empty;

    public string Verdict { get; set; } = string.Empty;

    public string DashboardHint { get; set; } = string.Empty;

    public string FindingsJson { get; set; } = "[]";

    public TariffBatchEntity Batch { get; set; } = null!;
}

public sealed class TradeDisputeEntity
{
    public Guid Id { get; set; }

    public Guid BatchId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public double TariffPercent { get; set; }

    public string Complaint { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Ruling { get; set; } = string.Empty;

    public bool HighlightAsError { get; set; }

    public string DashboardHint { get; set; } = string.Empty;

    public TariffBatchEntity Batch { get; set; } = null!;
}
