namespace Common.Rest.Address.Application.Services;

/// <summary>
/// Service for managing  address records with CRUD, search, and filter operations.
/// </summary>
public class AddressService(
    IRepository<AddressDocumentEntity> repository,
    IUnitOfWork unitOfWork,
    IAddressMappingService mappingService) : IAddressService
{
    private const string DocumentType = "Address";

    /// <summary>
    /// Creates a new address record.
    /// </summary>
    public async Task<AddressDocumentDto> CreateAddressAsync(CreateUpdateAddress createDto, string? userId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        var addressData = mappingService.MapToDomain(createDto);
        var document = new AddressDocumentEntity()
        {
            Id = Guid.NewGuid(),
            DocumentType = DocumentType,
            JsonData = addressData.JsonData,
            CreatedBy = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(document, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return mappingService.MapToDto(document);
    }

    /// <summary>
    /// Retrieves an address record by ID.
    /// </summary>
    public async Task<AddressDocumentDto?> GetAddressByIdAsync(Guid id, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        return document?.IsDeleted == true ? null : document is not null ? mappingService.MapToDto(document) : null;
    }

    /// <summary>
    /// Retrieves all active address records with pagination.
    /// </summary>
    public async Task<PaginationResult<AddressDocumentDto>> GetAllAddressesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var spec = new Specifications.AddressActiveSpecification(DocumentType);
        var (items, totalCount) = await repository.GetPagedAsync(
            page,
            pageSize,
            specification: spec,
            ct: ct);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<AddressDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Updates an existing address record.
    /// </summary>
    public async Task<AddressDocumentDto?> UpdateAddressAsync(Guid id, CreateUpdateAddress updateDto, string? userId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        var document = await repository.GetByIdAsync(id, ct);
        if (document is null || document.IsDeleted)
            return null;

        var updatedDomain = mappingService.UpdateDomain(document, updateDto);
        document.JsonData = updatedDomain.JsonData;
        document.DocumentType = updatedDomain.DocumentType;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync(ct);

        return mappingService.MapToDto(document);
    }

    /// <summary>
    /// Soft deletes an address record.
    /// </summary>
    public async Task<bool> DeleteAddressAsync(Guid id, string? userId = null, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        if (document is null || document.IsDeleted)
            return false;

        document.IsDeleted = true;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Permanently deletes an address record.
    /// </summary>
    public async Task<bool> PermanentlyDeleteAddressAsync(Guid id, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        if (document is null)
            return false;

        repository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Searches for addresses by UPRN (Unique Property Reference Number).
    /// </summary>
    public async Task<AddressDocumentDto?> GetAddressByUprnAsync(string uprn, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(uprn))
            return null;

        var spec = new Specifications.AddressUprnSpecification(DocumentType, uprn);
        var documents = await repository.FindAsync(spec, ct);
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
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var spec = new Specifications.AddressAdvancedSearchSpecification(
            DocumentType, postcode, postTown, organisation, thoroughfare, locality);
        var (items, totalCount) = await repository.GetPagedAsync(
            page,
            pageSize,
            specification: spec,
            ct: ct);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<AddressDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Gets the count of active addresses.
    /// </summary>
    public async Task<int> GetAddressCountAsync(CancellationToken ct = default)
    {
        var spec = new Specifications.AddressActiveSpecification(DocumentType);
        var all = await repository.FindAsync(spec, ct);
        return all.Count;
    }

    /// <summary>
    /// Restores a soft-deleted address record.
    /// </summary>
    public async Task<bool> RestoreAddressAsync(Guid id, string? userId = null, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        if (document is null || !document.IsDeleted)
            return false;

        document.IsDeleted = false;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.UpdatedBy = userId;

        repository.Update(document);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}
