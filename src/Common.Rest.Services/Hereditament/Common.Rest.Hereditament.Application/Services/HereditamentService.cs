using System.Net;

namespace Common.Rest.Hereditament.Application.Services;

/// <summary>
/// Service for managing  Hereditament records with CRUD, search, and filter operations.
/// </summary>
public class HereditamentService(
    IRepository<HereditamentDocumentEntity> repository,
    IUnitOfWork unitOfWork,
    IHereditamentMappingService mappingService) : IHereditamentService
{
    private const string DocumentType = "Hereditament";

    /// <summary>
    /// Creates a new Hereditament record.
    /// </summary>
    public async Task<HereditamentDocumentDto> CreateHereditamentAsync(CreateUpdateHereditament createDto, string? userId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        ///validate addressId exists in address service if provided
        if (!createDto.AddressId.HasValue || !IsValidAddress(createDto.AddressId))
        {
            createDto.Status = HereditamentStatus.Draft;
        }
        var HereditamentData = mappingService.MapToDomain(createDto);
        
        
        var document = new HereditamentDocumentEntity()
        {
            Id = Guid.NewGuid(),
            DocumentType = DocumentType,
            JsonData = HereditamentData.JsonData,
            CreatedBy = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(document, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return mappingService.MapToDto(document);
    }

    private bool IsValidAddress(Guid? addressId)
    {
        //validate addressId exists in address service
        // This is a placeholder for actual validation logic, which would typically involve an API call to the address service.
        return true;
    }

    /// <summary>
    /// Retrieves an Hereditament record by ID.
    /// </summary>
    public async Task<HereditamentDocumentDto?> GetHereditamentByIdAsync(Guid id, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        return document?.IsDeleted == true ? null : document is not null ? mappingService.MapToDto(document) : null;
    }

    /// <summary>
    /// Retrieves all active Hereditament records with pagination.
    /// </summary>
    public async Task<PaginationResult<HereditamentDocumentDto>> GetAllHereditamentesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var spec = new Specifications.HereditamentActiveSpecification(DocumentType);
        var (items, totalCount) = await repository.GetPagedAsync(
            page,
            pageSize,
            specification: spec,
            ct: ct);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<HereditamentDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Updates an existing Hereditament record.
    /// </summary>
    public async Task<HereditamentDocumentDto?> UpdateHereditamentAsync(Guid id, CreateUpdateHereditament updateDto, string? userId = null, CancellationToken ct = default)
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
    /// Soft deletes an Hereditament record.
    /// </summary>
    public async Task<bool> DeleteHereditamentAsync(Guid id, string? userId = null, CancellationToken ct = default)
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
    /// Permanently deletes an Hereditament record.
    /// </summary>
    public async Task<bool> PermanentlyDeleteHereditamentAsync(Guid id, CancellationToken ct = default)
    {
        var document = await repository.GetByIdAsync(id, ct);
        if (document is null)
            return false;

        repository.Remove(document);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Performs an advanced search with multiple filter criteria.
    /// </summary>
    public async Task<PaginationResult<HereditamentDocumentDto>> AdvancedSearchAsync(
         string? name = null,
        string? status = null,
        DateOnly? effectiveFrom = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var spec = new Specifications.HereditamentAdvancedSearchSpecification(
            DocumentType, name, status, effectiveFrom);
        var (items, totalCount) = await repository.GetPagedAsync(
            page,
            pageSize,
            specification: spec,
            ct: ct);

        var data = items.Select(mappingService.MapToDto).ToList();
        return new PaginationResult<HereditamentDocumentDto>(data, totalCount);
    }

    /// <summary>
    /// Gets the count of active Hereditamentes.
    /// </summary>
    public async Task<int> GetHereditamentCountAsync(CancellationToken ct = default)
    {
        var spec = new Specifications.HereditamentActiveSpecification(DocumentType);
        var all = await repository.FindAsync(spec, ct);
        return all.Count;
    }

    /// <summary>
    /// Restores a soft-deleted Hereditament record.
    /// </summary>
    public async Task<bool> RestoreHereditamentAsync(Guid id, string? userId = null, CancellationToken ct = default)
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