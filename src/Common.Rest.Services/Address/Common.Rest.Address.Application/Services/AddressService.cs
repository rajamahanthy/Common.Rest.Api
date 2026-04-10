namespace Common.Rest.Address.Application.Services;

using System.Linq.Expressions;
using Common.Rest.Address.Application.Dtos;
using Common.Rest.Address.Application.Interfaces;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Domain;
using Common.Rest.Shared.Repository;

/// <summary>
/// Service for managing  address records with CRUD, search, and filter operations.
/// </summary>
public class AddressService(
    IRepository<DocumentEntity<AddressEntity>> repository,
    IUnitOfWork unitOfWork,
    IAddressMappingService mappingService) : IAddressService
{
    private const string DocumentType = "AddressEntity";

    /// <summary>
    /// Creates a new address record.
    /// </summary>
    public async Task<AddressDocumentDto> CreateAddressAsync(CreateUpdateAddress createDto, string? userId = null)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        var addressData = mappingService.MapToDomain(createDto);
        var document = new DocumentEntity<AddressEntity>
        {
            Id = Guid.NewGuid(),
            DocumentType = DocumentType,
            JsonData = addressData.JsonData,
            CreatedBy = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(document);
        await unitOfWork.SaveChangesAsync();

        return mappingService.MapToDto(document);
    }

    /// <summary>
    /// Retrieves an address record by ID.
    /// </summary>
    public async Task<AddressDocumentDto?> GetAddressByIdAsync(Guid id)
    {
        var document = await repository.GetByIdAsync(id);
        return document?.IsDeleted == true ? null : document is not null ? mappingService.MapToDto(document) : null;
    }

    /// <summary>
    /// Retrieves all active address records with pagination.
    /// </summary>
    public async Task<PaginationResult<AddressDocumentDto>> GetAllAddressesAsync(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await repository.GetPagedAsync(
            page: page,
            pageSize: pageSize,
            predicate: d => !d.IsDeleted && d.DocumentType == DocumentType);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<AddressDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Updates an existing address record.
    /// </summary>
    public async Task<AddressDocumentDto?> UpdateAddressAsync(Guid id, CreateUpdateAddress updateDto, string? userId = null)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        var document = await repository.GetByIdAsync(id);
        if (document is null || document.IsDeleted)
            return null;

        var newDocument = mappingService.UpdateDomain(document, updateDto);
        document.JsonData = newDocument.JsonData;
        document.DocumentType = newDocument.DocumentType;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync();

        return mappingService.MapToDto(document);
    }

    /// <summary>
    /// Soft deletes an address record.
    /// </summary>
    public async Task<bool> DeleteAddressAsync(Guid id, string? userId = null)
    {
        var document = await repository.GetByIdAsync(id);
        if (document is null || document.IsDeleted)
            return false;

        document.IsDeleted = true;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Permanently deletes an address record.
    /// </summary>
    public async Task<bool> PermanentlyDeleteAddressAsync(Guid id)
    {
        var document = await repository.GetByIdAsync(id);
        if (document is null)
            return false;

        repository.Remove(document);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Searches for addresses by UPRN (Unique Property Reference Number).
    /// </summary>
    public async Task<AddressDocumentDto?> GetAddressByUprnAsync(string uprn)
    {
        if (string.IsNullOrWhiteSpace(uprn))
            return null;

        var documents = await repository.FindAsync(
            predicate: d => !d.IsDeleted && 
                           d.DocumentType == DocumentType && 
                           d.JsonData.Uprn == uprn);

        var document = documents.FirstOrDefault();
        return document is not null ? mappingService.MapToDto(document) : null;
    }

    /// <summary>
    /// Performs an advanced search with multiple filter criteria.
    /// </summary>
    public async Task<PaginationResult<AddressDocumentDto>> AdvancedSearchAsync(
        string? postcode = null,
        string? postTown = null,
        string? organisation = null,
        string? thoroughfare = null,
        string? locality = null,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        Expression<Func<DocumentEntity<AddressEntity>, bool>> predicate = d =>
            !d.IsDeleted && 
            d.DocumentType == DocumentType &&
            (string.IsNullOrWhiteSpace(postcode) || d.JsonData.AddressInfo.Postcode.Equals(postcode.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(postTown) || d.JsonData.AddressInfo.StreetDescriptor.PostTown.Contains(postTown.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(organisation) || (d.JsonData.AddressInfo.Organisation != null && d.JsonData.AddressInfo.Organisation.Contains(organisation.Trim(), StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrWhiteSpace(locality) || (d.JsonData.AddressInfo.StreetDescriptor.DependentLocality != null && d.JsonData.AddressInfo.StreetDescriptor.DependentLocality.Contains(locality.Trim(), StringComparison.OrdinalIgnoreCase)) 
            || (d.JsonData.AddressInfo.StreetDescriptor.DoubleDependentLocality != null && d.JsonData.AddressInfo.StreetDescriptor.DoubleDependentLocality.Contains(locality.Trim(), StringComparison.OrdinalIgnoreCase)));

        var (items, totalCount) = await repository.GetPagedAsync(
            page: page,
            pageSize: pageSize,
            predicate: predicate);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<AddressDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Gets the count of active addresses.
    /// </summary>
    public async Task<int> GetAddressCountAsync()
    {
        return await repository.CountAsync(
            predicate: d => !d.IsDeleted && d.DocumentType == DocumentType);
    }

    /// <summary>
    /// Restores a soft-deleted address record.
    /// </summary>
    public async Task<bool> RestoreAddressAsync(Guid id, string? userId = null)
    {
        var document = await repository.GetByIdAsync(id);
        if (document is null || !document.IsDeleted)
            return false;

        document.IsDeleted = false;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync();

        return true;
    }
}


