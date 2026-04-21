namespace Common.Rest.Hereditament.Application.Services;

/// <summary>
/// Implementation of mapping service for Hereditament entities.
/// Centralizes conversion logic between domain entities and DTOs.
/// </summary>
public class HereditamentMappingService : IHereditamentMappingService
{
    public HereditamentDocumentDto MapToDto(HereditamentDocumentEntity document)
    {
        ArgumentNullException.ThrowIfNull(document);
        
        var Hereditament = document.JsonData;
        ArgumentNullException.ThrowIfNull(Hereditament);  // Keep this (JsonData could be null)

        var HereditamentDto = new HereditamentDto
        {
            Name = Hereditament.Name,
            Status = Hereditament.Status,
            EffectiveFrom = Hereditament.EffectiveFrom,
            AddressId = Hereditament.AddressId
        };

        return new HereditamentDocumentDto(document.Id, HereditamentDto);
    }

    public HereditamentDocumentEntity MapToDomain(CreateUpdateHereditament createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        var hereditamentEntity = new HereditamentEntity
        {
            UARN = Guid.NewGuid(),
            Name = createDto.Name,
            Status = HereditamentStatus.Draft,
            EffectiveFrom = createDto.EffectiveFrom,
            AddressId = createDto.AddressId
        };

        return new HereditamentDocumentEntity 
        { 
            DocumentType = "Hereditament", 
            JsonData = hereditamentEntity 
        };
    }

    public HereditamentDocumentEntity UpdateDomain(HereditamentDocumentEntity HereditamentDocEntity, CreateUpdateHereditament updateDto)
    {
        ArgumentNullException.ThrowIfNull(HereditamentDocEntity);
        ArgumentNullException.ThrowIfNull(updateDto);
        ArgumentNullException.ThrowIfNull(HereditamentDocEntity.JsonData);

        var existing = HereditamentDocEntity.JsonData;

        var updatedEntity = new HereditamentEntity
        {
            UARN = existing.UARN,
            Name = updateDto.Name,
            Status = existing.Status,
            EffectiveFrom = updateDto.EffectiveFrom,
            AddressId = updateDto.AddressId
        };

        return new HereditamentDocumentEntity 
        { 
            DocumentType = "Hereditament", 
            JsonData = updatedEntity 
        };
    }
}
