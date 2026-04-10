namespace Common.Rest.Address.Application.Interfaces;

using Common.Rest.Address.Application.Dtos;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Domain;

/// <summary>
/// Mapping service for converting between domain entities, DTOs, and API models.
/// </summary>
public interface IAddressMappingService
{
    /// <summary>
    /// Maps a DocumentEntity to an AddressDto.
    /// </summary>
    AddressDocumentDto MapToDto(DocumentEntity<AddressEntity> document);

    /// <summary>
    /// Maps a CreateUpdateAddress DTO to a AddressEntity domain entity.
    /// </summary>
    DocumentEntity<AddressEntity> MapToDomain(CreateUpdateAddress createDto);

    /// <summary>
    /// Updates a AddressEntity with values from a CreateUpdateAddress DTO.
    /// </summary>
    DocumentEntity<AddressEntity> UpdateDomain(DocumentEntity<AddressEntity> addressDocEntity, CreateUpdateAddress updateDto);
}
