namespace Common.Rest.Address.Application.Interfaces;

/// <summary>
/// Domain service for managing address records.
/// </summary>
public interface IAddressService
{
    Task<TEntity?> GetByIdAsync<TEntity>(Guid id, CancellationToken ct = default) where TEntity : class;
    Task<AddressDto?> GetAddressByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedApiResponse<AddressDto>> GetPagedAddressesAsync(int page, int pageSize, string? searchTerm = null, CancellationToken ct = default);
    Task<AddressDto> CreateAddressAsync(CreateAddressRequest request, CancellationToken ct = default);
    Task UpdateAddressAsync(Guid id, UpdateAddressRequest request, CancellationToken ct = default);
    Task DeleteAddressAsync(Guid id, CancellationToken ct = default);
}
