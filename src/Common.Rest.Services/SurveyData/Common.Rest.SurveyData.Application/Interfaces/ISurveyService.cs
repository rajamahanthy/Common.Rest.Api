namespace Common.Rest.SurveyData.Application.Interfaces;

/// <summary>
/// Application service interface for Survey operations.
/// </summary>
public interface ISurveyService
{
    Task<SurveyDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SurveyDto> GetByReferenceAsync(string referenceNumber, CancellationToken ct = default);
    Task<(IReadOnlyList<SurveyDto> Items, int TotalCount)> SearchAsync(SurveySearchRequest request, CancellationToken ct = default);
    Task<SurveyDto> CreateAsync(CreateSurveyRequest request, string? userId = null, CancellationToken ct = default);
    Task<SurveyDto> UpdateAsync(Guid id, UpdateSurveyRequest request, string? userId = null, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
