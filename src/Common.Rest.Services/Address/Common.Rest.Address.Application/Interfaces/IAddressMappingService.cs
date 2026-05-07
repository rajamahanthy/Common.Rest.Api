

namespace Common.Rest.Address.Application.Interfaces;

/// <summary>
/// Mapping service for converting between domain entities, DTOs, and API models.
/// </summary>
public interface IAddressMappingService
{
    /// <summary>
    /// Maps an DocumentEntity<AddressEntity> to an AddressDto.
    /// </summary>
    AddressDocumentDto MapToDto(DocumentEntity<AddressEntity> document);

    /// <summary>
    /// Maps a CreateUpdateAddress DTO to an DocumentEntity<AddressEntity> domain entity.
    /// </summary>
    DocumentEntity<AddressEntity> MapToDomain(CreateUpdateAddress createDto);

    /// <summary>
    /// Updates an DocumentEntity<AddressEntity> with values from a CreateUpdateAddress DTO.
    /// </summary>
    DocumentEntity<AddressEntity> UpdateDomain(DocumentEntity<AddressEntity> addressDocEntity, CreateUpdateAddress updateDto);
}
