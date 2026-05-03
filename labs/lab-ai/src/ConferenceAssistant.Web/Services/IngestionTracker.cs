using ConferenceAssistant.Ingestion.Models;
using ConferenceAssistant.Ingestion.Services;
using ConferenceAssistant.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferenceAssistant.Web.Services;

public class IngestionTracker(IDbContextFactory<ConferenceDbContext> dbFactory) : IIngestionTracker
{
    public async Task<IngestionRecord?> GetRecordAsync(string documentId, string source)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.IngestionRecords
            .FirstOrDefaultAsync(r => r.DocumentId == documentId && r.Source == source);
    }

    public async Task UpsertRecordAsync(IngestionRecord record)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var existing = await db.IngestionRecords
            .FirstOrDefaultAsync(r => r.DocumentId == record.DocumentId && r.Source == record.Source);

        if (existing is not null)
        {
            existing.ContentHash = record.ContentHash;
            existing.Status = record.Status;
            existing.ErrorMessage = record.ErrorMessage;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            db.IngestionRecords.Add(record);
        }

        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<IngestionRecord>> GetRecordsForSourceAsync(string source)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.IngestionRecords
            .Where(r => r.Source == source)
            .ToListAsync();
    }
}
