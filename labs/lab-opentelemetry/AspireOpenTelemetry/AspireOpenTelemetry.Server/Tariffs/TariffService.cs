using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AspireOpenTelemetry.Server.Tariffs;

public sealed class TariffService(TariffDbContext dbContext, ILogger<TariffService> logger)
{
    private const int DefaultBatchSize = 8;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly CountryProfile[] Catalog =
    [
        new("AR", "Argentina", "competitive tango seminars", "a severe shortage of ceremonial glitter", 40),
        new("CA", "Canada", "polite lumber and apology futures", "an emergency maple balancing levy", 32),
        new("IS", "Iceland", "geothermal spa coupons", "volcanic jazz inflation", 58),
        new("JP", "Japan", "precision karaoke robots", "weekend joystick speculation", 38),
        new("KE", "Kenya", "championship coffee and dramatic sunrises", "very serious giraffe toll analytics", 42),
        new("NO", "Norway", "fjord reflections and premium sweaters", "a salmon optimism bubble", 35),
        new("NZ", "New Zealand", "cloud-soft sheep logistics", "an overachieving hobbit corridor fee", 46),
        new("PE", "Peru", "mystery potatoes and mountain gossip", "llama-powered customs turbulence", 48),
        new("SG", "Singapore", "disciplined snacks and shipping wizardry", "hyper-efficient gum suspense", 36),
        new("CH", "Switzerland", "extremely punctual chocolate", "alpine yodel surcharge futures", 44)
    ];

    private static readonly string[] Scenarios =
    [
        "Midnight accordion shortage",
        "Executive whimsy summit",
        "Moonbeam retaliation week",
        "Emergency glitter stabilization plan",
        "Unexpected spreadsheet thunderstorm"
    ];

    private static readonly string[] MinisterNotes =
    [
        "Approved after one dramatic gasp and a very small gong.",
        "Rubber-stamped by the deputy minister for vibes.",
        "Passed unanimously by the council of overconfident spreadsheets.",
        "Signed with a fountain pen full of sparkling panic.",
        "Declared fiscally necessary by a panel of suspiciously cheerful interns."
    ];

    private static readonly string[] Complaints =
    [
        "Our ceremonial biscuits now cost more than a lighthouse.",
        "This tariff has frightened the accountants and one emotional forklift.",
        "The surcharge appears to have been rounded using interpretive dance.",
        "A goose from customs keeps whispering bigger numbers."
    ];

    public async Task<TariffBatchResponse> GetCurrentBatchAsync(CancellationToken cancellationToken)
    {
        TariffTelemetry.UpdateCatalogSize(Catalog.Length);

        var latestBatch = await GetLatestBatchEntityAsync(cancellationToken);
        if (latestBatch is null)
        {
            return await GenerateBatchAsync(DefaultBatchSize, null, cancellationToken);
        }

        return await BuildBatchResponseAsync(latestBatch, cancellationToken);
    }

    public async Task<TariffBatchResponse> GenerateBatchAsync(int count, string? scenario, CancellationToken cancellationToken)
    {
        TariffTelemetry.UpdateCatalogSize(Catalog.Length);

        var normalizedCount = Math.Clamp(count, 4, Catalog.Length);
        var scenarioName = string.IsNullOrWhiteSpace(scenario)
            ? Scenarios[Random.Shared.Next(Scenarios.Length)]
            : scenario.Trim();
        var generatedAt = DateTimeOffset.UtcNow;
        var batchId = Guid.NewGuid();

        using var activity = TariffTelemetry.ActivitySource.StartActivity("tariff.generate.batch", ActivityKind.Internal);
        activity?.SetTag("tariff.batch.id", batchId);
        activity?.SetTag("tariff.scenario", scenarioName);
        activity?.SetTag("tariff.country.count", normalizedCount);
        activity?.SetTag("db.system", "postgresql");
        activity?.AddEvent(new ActivityEvent("tariff.batch.requested"));

        var tariffs = Catalog
            .OrderBy(_ => Random.Shared.Next())
            .Take(normalizedCount)
            .Select(country => CreateTariff(country, scenarioName))
            .OrderByDescending(tariff => tariff.TariffPercent)
            .ToArray();

        foreach (var tariff in tariffs)
        {
            var tags = new TagList
            {
                { "country.code", tariff.CountryCode },
                { "risk.level", tariff.RiskLevel },
                { "scenario", scenarioName }
            };

            TariffTelemetry.CountryQuotesGenerated.Add(1, tags);
            TariffTelemetry.TariffPercent.Record(tariff.TariffPercent, tags);

            activity?.AddEvent(new ActivityEvent(
                "tariff.country.calculated",
                tags: new ActivityTagsCollection
                {
                    { "country.code", tariff.CountryCode },
                    { "country.name", tariff.CountryName },
                    { "tariff.percent", tariff.TariffPercent },
                    { "tariff.risk_level", tariff.RiskLevel }
                }));
        }

        var absurdityIndex = (int)Math.Round(tariffs.Average(tariff => tariff.AbsurdityScore));
        var averageTariff = Math.Round(tariffs.Average(tariff => tariff.TariffPercent), 1);
        var highestTariff = tariffs[0];

        var batchEntity = new TariffBatchEntity
        {
            Id = batchId,
            Scenario = scenarioName,
            Headline = $"{highestTariff.CountryName} now faces a {highestTariff.TariffBand.ToLowerInvariant()} thanks to {scenarioName.ToLowerInvariant()}.",
            GeneratedAt = generatedAt,
            AbsurdityIndex = absurdityIndex,
            AverageTariff = averageTariff,
            Quotes = tariffs.Select(tariff => new TariffQuoteEntity
            {
                Id = Guid.NewGuid(),
                BatchId = batchId,
                CountryCode = tariff.CountryCode,
                CountryName = tariff.CountryName,
                ExportSpecialty = tariff.ExportSpecialty,
                TariffPercent = tariff.TariffPercent,
                TariffBand = tariff.TariffBand,
                AbsurdityScore = tariff.AbsurdityScore,
                RiskLevel = tariff.RiskLevel,
                Volatility = tariff.Volatility,
                SillyReason = tariff.SillyReason,
                MinisterNote = tariff.MinisterNote
            }).ToList()
        };

        dbContext.TariffBatches.Add(batchEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        TariffTelemetry.UpdateAbsurdityIndex(absurdityIndex);
        TariffTelemetry.BatchGenerated.Add(1, new TagList { { "scenario", scenarioName } });
        TariffTelemetry.BatchSize.Record(tariffs.Length, new TagList { { "scenario", scenarioName } });

        logger.LogInformation(
            "Generated batch {BatchId} with {CountryCount} absurd tariffs for scenario {Scenario} and absurdity index {AbsurdityIndex}",
            batchId,
            tariffs.Length,
            scenarioName,
            absurdityIndex);

        if (highestTariff.TariffPercent >= 220)
        {
            logger.LogWarning(
                "Tariff batch {BatchId} peaked at {TariffPercent} percent for {CountryName} during scenario {Scenario}",
                batchId,
                highestTariff.TariffPercent,
                highestTariff.CountryName,
                scenarioName);
        }

        activity?.SetTag("tariff.average_percent", averageTariff);
        activity?.SetTag("tariff.absurdity_index", absurdityIndex);
        activity?.SetTag("tariff.highest_country", highestTariff.CountryCode);
        activity?.SetTag("tariff.highest_percent", highestTariff.TariffPercent);
        activity?.AddEvent(new ActivityEvent("tariff.batch.persisted"));

        return new TariffBatchResponse(
            batchId,
            scenarioName,
            batchEntity.Headline,
            generatedAt,
            absurdityIndex,
            averageTariff,
            LatestAudit: null,
            LatestDispute: null,
            Tariffs: tariffs);
    }

    public async Task<TariffInspectionResponse?> InspectCountryAsync(string countryCode, CancellationToken cancellationToken)
    {
        var latestBatch = await GetLatestBatchEntityAsync(cancellationToken);
        if (latestBatch is null)
        {
            latestBatch = await EnsureBatchExistsAsync(cancellationToken);
        }

        var tariff = latestBatch.Quotes
            .Select(MapCard)
            .FirstOrDefault(t => string.Equals(t.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase));

        if (tariff is null)
        {
            logger.LogWarning("Attempted to inspect unknown tariff for country code {CountryCode}", countryCode);
            return null;
        }

        using var activity = TariffTelemetry.ActivitySource.StartActivity("tariff.inspect.country", ActivityKind.Internal);
        activity?.SetTag("tariff.batch.id", latestBatch.Id);
        activity?.SetTag("country.code", tariff.CountryCode);
        activity?.SetTag("country.name", tariff.CountryName);
        activity?.SetTag("tariff.percent", tariff.TariffPercent);
        activity?.SetTag("tariff.risk_level", tariff.RiskLevel);
        activity?.SetTag("db.system", "postgresql");
        activity?.AddEvent(new ActivityEvent("tariff.country.reviewed"));

        TariffTelemetry.InspectionRequests.Add(1, new TagList
        {
            { "country.code", tariff.CountryCode },
            { "risk.level", tariff.RiskLevel }
        });

        logger.LogInformation(
            "Inspected batch {BatchId} tariff for {CountryName} at {TariffPercent} percent in band {TariffBand}",
            latestBatch.Id,
            tariff.CountryName,
            tariff.TariffPercent,
            tariff.TariffBand);

        return new TariffInspectionResponse(
            latestBatch.Id,
            tariff,
            $"{tariff.CountryName} was taxed because {tariff.SillyReason.ToLowerInvariant()}",
            "Open the trace for this request and inspect the tariff.country.reviewed event on the custom span.",
            "This request writes a structured log with the batch, country, tariff percent, and band.",
            "This request increments the tariff.inspect.requests counter for the selected country.");
    }

    public async Task<TariffAuditResponse> RunAuditAsync(string? focusCountryCode, CancellationToken cancellationToken)
    {
        var latestBatch = await GetLatestBatchEntityAsync(cancellationToken) ?? await EnsureBatchExistsAsync(cancellationToken);
        var latestCards = latestBatch.Quotes.Select(MapCard).OrderByDescending(tariff => tariff.TariffPercent).ToArray();

        var focusTariff = latestCards.FirstOrDefault(t =>
            string.Equals(t.CountryCode, focusCountryCode, StringComparison.OrdinalIgnoreCase));
        var flaggedTariffs = latestCards
            .Where(tariff => tariff.TariffPercent >= 180 || tariff.AbsurdityScore >= 85)
            .OrderByDescending(tariff => tariff.TariffPercent)
            .ToArray();
        var highestTariff = latestCards[0];
        var findings = BuildAuditFindings(flaggedTariffs, focusTariff, highestTariff);
        var auditId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var verdict = flaggedTariffs.Length == 0
            ? "The tariff board is still nonsense, but technically calm."
            : "The ministry has entered a measurable state of glitter-powered concern.";
        const string dashboardHint = "Check the audit trace for tariff.audit.flagged events and the metrics for tariff.audit.runs plus tariff.audit.flagged_countries.";

        using var activity = TariffTelemetry.ActivitySource.StartActivity("tariff.audit.run", ActivityKind.Internal);
        activity?.SetTag("tariff.audit.id", auditId);
        activity?.SetTag("tariff.batch.id", latestBatch.Id);
        activity?.SetTag("tariff.scenario", latestBatch.Scenario);
        activity?.SetTag("tariff.flagged_count", flaggedTariffs.Length);
        activity?.SetTag("tariff.focus_country", focusTariff?.CountryCode ?? "none");
        activity?.SetTag("tariff.highest_country", highestTariff.CountryCode);
        activity?.SetTag("db.system", "postgresql");
        activity?.AddEvent(new ActivityEvent("tariff.audit.started"));

        foreach (var flaggedTariff in flaggedTariffs.Take(3))
        {
            activity?.AddEvent(new ActivityEvent(
                "tariff.audit.flagged",
                tags: new ActivityTagsCollection
                {
                    { "country.code", flaggedTariff.CountryCode },
                    { "country.name", flaggedTariff.CountryName },
                    { "tariff.percent", flaggedTariff.TariffPercent }
                }));
        }

        var auditEntity = new AuditRunEntity
        {
            Id = auditId,
            BatchId = latestBatch.Id,
            CreatedAt = createdAt,
            FocusCountryCode = focusTariff?.CountryCode,
            FocusCountryName = focusTariff?.CountryName,
            FlaggedCountries = flaggedTariffs.Length,
            AverageTariff = latestBatch.AverageTariff,
            HighestCountryCode = highestTariff.CountryCode,
            HighestCountryName = highestTariff.CountryName,
            HighestExportSpecialty = highestTariff.ExportSpecialty,
            HighestTariffPercent = highestTariff.TariffPercent,
            HighestTariffBand = highestTariff.TariffBand,
            HighestAbsurdityScore = highestTariff.AbsurdityScore,
            HighestRiskLevel = highestTariff.RiskLevel,
            HighestVolatility = highestTariff.Volatility,
            HighestSillyReason = highestTariff.SillyReason,
            HighestMinisterNote = highestTariff.MinisterNote,
            Verdict = verdict,
            DashboardHint = dashboardHint,
            FindingsJson = JsonSerializer.Serialize(findings, JsonOptions)
        };

        dbContext.AuditRuns.Add(auditEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        TariffTelemetry.AuditRuns.Add(1, new TagList { { "scenario", latestBatch.Scenario } });
        TariffTelemetry.FlaggedCountries.Add(flaggedTariffs.Length, new TagList { { "scenario", latestBatch.Scenario } });

        logger.LogInformation(
            "Emergency tariff audit {AuditId} for batch {BatchId} in scenario {Scenario} flagged {FlaggedCountries} countries",
            auditId,
            latestBatch.Id,
            latestBatch.Scenario,
            flaggedTariffs.Length);

        if (flaggedTariffs.Length > 0)
        {
            logger.LogWarning(
                "Tariff audit {AuditId} found dramatic outliers led by {CountryName} at {TariffPercent} percent",
                auditId,
                highestTariff.CountryName,
                highestTariff.TariffPercent);
        }

        activity?.AddEvent(new ActivityEvent("tariff.audit.persisted"));

        return new TariffAuditResponse(
            auditId,
            latestBatch.Id,
            createdAt,
            latestBatch.Scenario,
            focusTariff?.CountryCode,
            focusTariff?.CountryName,
            flaggedTariffs.Length,
            latestBatch.AverageTariff,
            highestTariff,
            verdict,
            findings,
            dashboardHint);
    }

    public async Task<TradeDisputeResponse> ResolveDisputeAsync(string? countryCode, string? complaint, CancellationToken cancellationToken)
    {
        var latestBatch = await GetLatestBatchEntityAsync(cancellationToken) ?? await EnsureBatchExistsAsync(cancellationToken);
        var tariffs = latestBatch.Quotes.Select(MapCard).ToArray();
        var tariff = SelectTariffForDispute(tariffs, countryCode);
        var complaintText = string.IsNullOrWhiteSpace(complaint)
            ? Complaints[Random.Shared.Next(Complaints.Length)]
            : complaint.Trim();
        var disputeId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var highlightAsError = tariff.TariffPercent >= 210 || tariff.RiskLevel is "Volcanic" or "Catastrophic";
        var severity = highlightAsError ? "error" : "warning";
        var outcome = highlightAsError
            ? "Escalated to the Department of Dramatic Overreaction."
            : "Settled over ceremonial biscuits and one cautious handshake.";
        var ruling = highlightAsError
            ? $"{tariff.CountryName} was deemed too economically theatrical for same-day diplomacy."
            : $"{tariff.CountryName} accepted a temporary friendship coupon and a discount on glitter storage.";
        var dashboardHint = highlightAsError
            ? "Open the trace to find the error-status custom span and correlate it with the warning log."
            : "Open the dispute trace to inspect the custom span events and correlated information log.";

        using var activity = TariffTelemetry.ActivitySource.StartActivity("tariff.dispute.resolve", ActivityKind.Internal);
        activity?.SetTag("tariff.dispute.id", disputeId);
        activity?.SetTag("tariff.batch.id", latestBatch.Id);
        activity?.SetTag("country.code", tariff.CountryCode);
        activity?.SetTag("country.name", tariff.CountryName);
        activity?.SetTag("tariff.percent", tariff.TariffPercent);
        activity?.SetTag("tariff.dispute.severity", severity);
        activity?.SetTag("tariff.dispute.complaint", complaintText);
        activity?.SetTag("db.system", "postgresql");
        activity?.AddEvent(new ActivityEvent("tariff.dispute.received"));
        activity?.AddEvent(new ActivityEvent(highlightAsError ? "tariff.dispute.escalated" : "tariff.dispute.resolved"));
        activity?.SetStatus(highlightAsError ? ActivityStatusCode.Error : ActivityStatusCode.Ok);

        var tags = new TagList
        {
            { "country.code", tariff.CountryCode },
            { "severity", severity }
        };

        var disputeEntity = new TradeDisputeEntity
        {
            Id = disputeId,
            BatchId = latestBatch.Id,
            CreatedAt = createdAt,
            CountryCode = tariff.CountryCode,
            CountryName = tariff.CountryName,
            TariffPercent = tariff.TariffPercent,
            Complaint = complaintText,
            Outcome = outcome,
            Severity = severity,
            Ruling = ruling,
            HighlightAsError = highlightAsError,
            DashboardHint = dashboardHint
        };

        dbContext.TradeDisputes.Add(disputeEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        TariffTelemetry.DisputeAttempts.Add(1, tags);

        if (highlightAsError)
        {
            TariffTelemetry.DisputeEscalations.Add(1, tags);
            logger.LogWarning(
                "Trade dispute {DisputeId} for batch {BatchId} escalated for {CountryName} with complaint {Complaint} at {TariffPercent} percent",
                disputeId,
                latestBatch.Id,
                tariff.CountryName,
                complaintText,
                tariff.TariffPercent);
        }
        else
        {
            logger.LogInformation(
                "Trade dispute {DisputeId} for batch {BatchId} settled for {CountryName} with complaint {Complaint}",
                disputeId,
                latestBatch.Id,
                tariff.CountryName,
                complaintText);
        }

        activity?.AddEvent(new ActivityEvent("tariff.dispute.persisted"));

        return new TradeDisputeResponse(
            disputeId,
            latestBatch.Id,
            createdAt,
            tariff.CountryCode,
            tariff.CountryName,
            tariff.TariffPercent,
            complaintText,
            outcome,
            severity,
            ruling,
            highlightAsError,
            dashboardHint);
    }

    private async Task<TariffBatchEntity> EnsureBatchExistsAsync(CancellationToken cancellationToken)
    {
        var response = await GenerateBatchAsync(DefaultBatchSize, null, cancellationToken);
        var batch = await dbContext.TariffBatches
            .Include(entity => entity.Quotes)
            .OrderByDescending(entity => entity.GeneratedAt)
            .FirstAsync(entity => entity.Id == response.BatchId, cancellationToken);

        return batch;
    }

    private async Task<TariffBatchEntity?> GetLatestBatchEntityAsync(CancellationToken cancellationToken) =>
        await dbContext.TariffBatches
            .Include(batch => batch.Quotes)
            .AsNoTracking()
            .OrderByDescending(batch => batch.GeneratedAt)
            .FirstOrDefaultAsync(cancellationToken);

    private async Task<TariffBatchResponse> BuildBatchResponseAsync(TariffBatchEntity batch, CancellationToken cancellationToken)
    {
        var latestAudit = await dbContext.AuditRuns
            .AsNoTracking()
            .Where(audit => audit.BatchId == batch.Id)
            .OrderByDescending(audit => audit.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var latestDispute = await dbContext.TradeDisputes
            .AsNoTracking()
            .Where(dispute => dispute.BatchId == batch.Id)
            .OrderByDescending(dispute => dispute.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        TariffTelemetry.UpdateAbsurdityIndex(batch.AbsurdityIndex);

        return new TariffBatchResponse(
            batch.Id,
            batch.Scenario,
            batch.Headline,
            batch.GeneratedAt,
            batch.AbsurdityIndex,
            batch.AverageTariff,
            MapAuditSummary(latestAudit),
            MapDisputeSummary(latestDispute),
            batch.Quotes
                .OrderByDescending(quote => quote.TariffPercent)
                .Select(MapCard)
                .ToArray());
    }

    private static TariffAuditSummary? MapAuditSummary(AuditRunEntity? audit) =>
        audit is null
            ? null
            : new TariffAuditSummary(
                audit.Id,
                audit.CreatedAt,
                audit.FocusCountryCode,
                audit.FocusCountryName,
                audit.FlaggedCountries,
                audit.Verdict,
                audit.DashboardHint);

    private static TradeDisputeSummary? MapDisputeSummary(TradeDisputeEntity? dispute) =>
        dispute is null
            ? null
            : new TradeDisputeSummary(
                dispute.Id,
                dispute.CreatedAt,
                dispute.CountryCode,
                dispute.CountryName,
                dispute.Severity,
                dispute.Outcome,
                dispute.HighlightAsError);

    private static TariffCard MapCard(TariffQuoteEntity quote) =>
        new(
            quote.CountryCode,
            quote.CountryName,
            quote.ExportSpecialty,
            quote.TariffPercent,
            quote.TariffBand,
            quote.AbsurdityScore,
            quote.RiskLevel,
            quote.Volatility,
            quote.SillyReason,
            quote.MinisterNote);

    private static TariffCard MapCard(AuditRunEntity audit) =>
        new(
            audit.HighestCountryCode,
            audit.HighestCountryName,
            audit.HighestExportSpecialty,
            audit.HighestTariffPercent,
            audit.HighestTariffBand,
            audit.HighestAbsurdityScore,
            audit.HighestRiskLevel,
            audit.HighestVolatility,
            audit.HighestSillyReason,
            audit.HighestMinisterNote);

    private static TariffCard CreateTariff(CountryProfile country, string scenario)
    {
        var tariffPercent = Math.Round(
            (double)(
                Random.Shared.Next(24, 96)
                + country.AbsurdityBias
                + Random.Shared.Next(18, 160)
                + GetScenarioBoost(scenario)),
            1);

        var absurdityScore = Math.Clamp(
            (int)Math.Round((tariffPercent / 3) + Random.Shared.Next(8, 26)),
            25,
            100);

        var riskLevel = tariffPercent switch
        {
            >= 260 => "Catastrophic",
            >= 210 => "Volcanic",
            >= 160 => "Spicy",
            >= 110 => "Bold",
            _ => "Playful"
        };

        var tariffBand = tariffPercent switch
        {
            >= 260 => "Emergency glitter wall",
            >= 210 => "Panic surcharge",
            >= 160 => "Chaotic levy",
            >= 110 => "Suspicious markup",
            _ => "Friendly nuisance"
        };

        var volatility = Random.Shared.NextDouble() switch
        {
            < 0.25 => "stable-ish",
            < 0.5 => "wobbly",
            < 0.75 => "chaotic",
            _ => "fully feral"
        };

        return new TariffCard(
            country.CountryCode,
            country.CountryName,
            country.ExportSpecialty,
            tariffPercent,
            tariffBand,
            absurdityScore,
            riskLevel,
            volatility,
            $"{scenario} collided with {country.BureaucraticTrigger}, so {country.ExportSpecialty} drew immediate suspicion.",
            MinisterNotes[Random.Shared.Next(MinisterNotes.Length)]);
    }

    private static int GetScenarioBoost(string scenario) =>
        scenario switch
        {
            "Midnight accordion shortage" => 36,
            "Executive whimsy summit" => 28,
            "Moonbeam retaliation week" => 44,
            "Emergency glitter stabilization plan" => 52,
            _ => 34
        };

    private static IReadOnlyList<string> BuildAuditFindings(
        IReadOnlyList<TariffCard> flaggedTariffs,
        TariffCard? focusTariff,
        TariffCard highestTariff)
    {
        var findings = new List<string>
        {
            $"{highestTariff.CountryName} currently leads the chaos board at {highestTariff.TariffPercent}%."
        };

        if (focusTariff is not null)
        {
            findings.Add($"{focusTariff.CountryName} is being watched because its volatility is {focusTariff.Volatility}.");
        }

        if (flaggedTariffs.Count == 0)
        {
            findings.Add("No country crossed the official panic glitter threshold during this audit.");
            return findings;
        }

        findings.AddRange(flaggedTariffs.Take(2).Select(tariff =>
            $"{tariff.CountryName} was flagged with a {tariff.TariffBand.ToLowerInvariant()} at {tariff.TariffPercent}%."));

        return findings;
    }

    private static TariffCard SelectTariffForDispute(IReadOnlyList<TariffCard> tariffs, string? countryCode)
    {
        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            var selected = tariffs.FirstOrDefault(t =>
                string.Equals(t.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase));

            if (selected is not null)
            {
                return selected;
            }
        }

        return tariffs[Random.Shared.Next(tariffs.Count)];
    }
}
