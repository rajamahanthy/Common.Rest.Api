namespace Common.Rest.Hereditament.Application.Interfaces;

/// <summary>
/// Domain service for managing  Hereditament records.
/// </summary>
public interface IHereditamentService
{
    Task<HereditamentDocumentDto> CreateHereditamentAsync(CreateUpdateHereditament createDto, string? userId = null, CancellationToken ct = default);
    Task<HereditamentDocumentDto?> GetHereditamentByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginationResult<HereditamentDocumentDto>> GetAllHereditamentesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<HereditamentDocumentDto?> UpdateHereditamentAsync(Guid id, CreateUpdateHereditament updateDto, string? userId = null, CancellationToken ct = default);
    Task<bool> DeleteHereditamentAsync(Guid id, string? userId = null, CancellationToken ct = default);
    Task<bool> PermanentlyDeleteHereditamentAsync(Guid id, CancellationToken ct = default);
    Task<PaginationResult<HereditamentDocumentDto>> AdvancedSearchAsync(
        string? name = null,
        string? status = null,
        DateOnly? effectiveFrom = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);
    Task<int> GetHereditamentCountAsync(CancellationToken ct = default);
    Task<bool> RestoreHereditamentAsync(Guid id, string? userId = null, CancellationToken ct = default);
}

