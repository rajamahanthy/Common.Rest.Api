namespace Common.Rest.Address.Application.Interfaces;

/// <summary>
/// Mapping service for converting between domain entities, DTOs, and API models.
/// </summary>
public interface IAddressMappingService
{
    /// <summary>
    /// Maps an AddressDocumentEntity to an AddressDto.
    /// </summary>
    AddressDocumentDto MapToDto(AddressDocumentEntity document);

    /// <summary>
    /// Maps a CreateUpdateAddress DTO to an AddressDocumentEntity domain entity.
    /// </summary>
    AddressDocumentEntity MapToDomain(CreateUpdateAddress createDto);

    /// <summary>
    /// Updates an AddressDocumentEntity with values from a CreateUpdateAddress DTO.
    /// </summary>
    AddressDocumentEntity UpdateDomain(AddressDocumentEntity addressDocEntity, CreateUpdateAddress updateDto);
}
