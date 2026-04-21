namespace Common.Rest.Hereditament.Application.Interfaces;

/// <summary>
/// Mapping service for converting between domain entities, DTOs, and API models.
/// </summary>
public interface IHereditamentMappingService
{
    /// <summary>
    /// Maps an HereditamentDocumentEntity to an HereditamentDto.
    /// </summary>
    HereditamentDocumentDto MapToDto(HereditamentDocumentEntity document);

    /// <summary>
    /// Maps a CreateUpdateHereditament DTO to an HereditamentDocumentEntity domain entity.
    /// </summary>
    HereditamentDocumentEntity MapToDomain(CreateUpdateHereditament createDto);

    /// <summary>
    /// Updates an HereditamentDocumentEntity with values from a CreateUpdateHereditament DTO.
    /// </summary>
    HereditamentDocumentEntity UpdateDomain(HereditamentDocumentEntity HereditamentDocEntity, CreateUpdateHereditament updateDto);
}
