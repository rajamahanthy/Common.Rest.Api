using RestApi.Shared.Repository;

namespace SurveyData.Infrastructure.Persistence;

/// <summary>
/// Survey-specific repository with eager loading for navigation properties.
/// </summary>
public sealed class SurveyRepository(SurveyDbContext context) : EfRepository<Survey>(context), ISurveyRepository
{
    public async Task<Survey?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken ct = default)
    {
        return await DbSet
            .Include(s => s.Details.OrderBy(d => d.SortOrder))
            .FirstOrDefaultAsync(s => s.ReferenceNumber == referenceNumber, ct);
    }

    public async Task<Survey?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(s => s.Details.OrderBy(d => d.SortOrder))
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
