namespace Common.Rest.Address.Application.Interfaces;

/// <summary>
/// Domain service for managing  address records.
/// </summary>
public interface IAddressService
{
    Task<AddressDocumentDto> CreateAddressAsync(CreateUpdateAddress createDto, string? userId = null, CancellationToken ct = default);
    Task<AddressDocumentDto?> GetAddressByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginationResult<AddressDocumentDto>> GetAllAddressesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<AddressDocumentDto?> UpdateAddressAsync(Guid id, CreateUpdateAddress updateDto, string? userId = null, CancellationToken ct = default);
    Task<bool> DeleteAddressAsync(Guid id, string? userId = null, CancellationToken ct = default);
    Task<bool> PermanentlyDeleteAddressAsync(Guid id, CancellationToken ct = default);
    Task<AddressDocumentDto?> GetAddressByUprnAsync(string uprn, CancellationToken ct = default);
    Task<PaginationResult<AddressDocumentDto>> AdvancedSearchAsync(
        string? postcode = null,
        string? postTown = null,
        string? organisation = null,
        string? thoroughfare = null,
        string? locality = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default);
    Task<int> GetAddressCountAsync(CancellationToken ct = default);
    Task<bool> RestoreAddressAsync(Guid id, string? userId = null, CancellationToken ct = default);
}

