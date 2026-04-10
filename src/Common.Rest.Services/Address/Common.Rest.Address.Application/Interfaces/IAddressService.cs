namespace Common.Rest.Address.Application.Interfaces;

using Common.Rest.Address.Application.Dtos;

/// <summary>
/// Domain service for managing  address records.
/// </summary>
public interface IAddressService
{
    Task<AddressDocumentDto> CreateAddressAsync(CreateUpdateAddress createDto, string? userId = null);
    Task<AddressDocumentDto?> GetAddressByIdAsync(Guid id);
    Task<PaginationResult<AddressDocumentDto>> GetAllAddressesAsync(int page = 1, int pageSize = 10);
    Task<AddressDocumentDto?> UpdateAddressAsync(Guid id, CreateUpdateAddress updateDto, string? userId = null);
    Task<bool> DeleteAddressAsync(Guid id, string? userId = null);
    Task<bool> PermanentlyDeleteAddressAsync(Guid id);
    Task<AddressDocumentDto?> GetAddressByUprnAsync(string uprn);
    Task<PaginationResult<AddressDocumentDto>> AdvancedSearchAsync(
        string? postcode = null,
        string? postTown = null,
        string? organisation = null,
        string? thoroughfare = null,
        string? locality = null,
        int page = 1,
        int pageSize = 10);
    Task<int> GetAddressCountAsync();
    Task<bool> RestoreAddressAsync(Guid id, string? userId = null);
}

