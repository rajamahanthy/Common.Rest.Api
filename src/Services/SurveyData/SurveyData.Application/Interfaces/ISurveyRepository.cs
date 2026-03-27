using RestApi.Shared.Repository;

namespace SurveyData.Application.Interfaces;

/// <summary>
/// Survey-specific repository with domain query methods.
/// </summary>
public interface ISurveyRepository : IRepository<Survey>
{
    Task<Survey?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken ct = default);
    Task<Survey?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
}
