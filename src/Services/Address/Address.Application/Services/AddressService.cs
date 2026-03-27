using Microsoft.EntityFrameworkCore;
using RestApi.Shared.Repository;

namespace Address.Application.Services;

public class AddressService(
    IRepository<Address.Domain.Entities.Address> repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IAddressService
{
    public async Task<TEntity?> GetByIdAsync<TEntity>(Guid id, CancellationToken ct = default) where TEntity : class
    {
        // This helper for generic base
        return await (repository as EfRepository<Address.Domain.Entities.Address>)!.Context.Set<TEntity>().FindAsync([id], ct);
    }

    public async Task<AddressDto?> GetAddressByIdAsync(Guid id, CancellationToken ct = default)
    {
        var address = await repository.GetByIdAsync(id, ct);
        return address is null ? null : mapper.Map<AddressDto>(address);
    }

    public async Task<PagedApiResponse<AddressDto>> GetPagedAddressesAsync(
        int page, int pageSize,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var (items, total) = await repository.GetPagedAsync(
            page, pageSize,
            predicate: a => string.IsNullOrEmpty(searchTerm) || a.SingleLineAddress.Contains(searchTerm) || a.Postcode.Contains(searchTerm),
            orderBy: a => a.CreatedAt,
            descending: true,
            ct: ct);

        var dtos = mapper.Map<IReadOnlyList<AddressDto>>(items);
        return new PagedApiResponse<AddressDto>(dtos, page, pageSize, total);
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressRequest request, CancellationToken ct = default)
    {
        var address = mapper.Map<Address.Domain.Entities.Address>(request);
        await repository.AddAsync(address, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<AddressDto>(address);
    }

    public async Task UpdateAddressAsync(Guid id, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await repository.GetByIdAsync(id, ct);
        if (address is null) return;

        mapper.Map(request, address);
        address.UpdatedAt = DateTimeOffset.UtcNow;
        
        repository.Update(address);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAddressAsync(Guid id, CancellationToken ct = default)
    {
        var address = await repository.GetByIdAsync(id, ct);
        if (address is null) return;

        address.IsDeleted = true;
        address.UpdatedAt = DateTimeOffset.UtcNow;
        
        repository.Update(address);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
