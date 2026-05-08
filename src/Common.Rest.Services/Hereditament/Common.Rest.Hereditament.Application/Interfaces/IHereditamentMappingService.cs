namespace Common.Rest.Hereditament.Application.Interfaces;

/// <summary>
/// Mapping service for converting between domain entities, DTOs, and API models.
/// </summary>
public interface IHereditamentMappingService
{
    /// <summary>
    /// Maps an  DocumentEntity<HereditamentEntity> to an HereditamentDto.
    /// </summary>
    HereditamentDocumentDto MapToDto( DocumentEntity<HereditamentEntity> document);

    /// <summary>
    /// Maps a CreateUpdateHereditament DTO to an  DocumentEntity<HereditamentEntity> domain entity.
    /// </summary>
     DocumentEntity<HereditamentEntity> MapToDomain(CreateUpdateHereditament createDto);

    /// <summary>
    /// Updates an  DocumentEntity<HereditamentEntity> with values from a CreateUpdateHereditament DTO.
    /// </summary>
     DocumentEntity<HereditamentEntity> UpdateDomain( DocumentEntity<HereditamentEntity> HereditamentDocEntity, CreateUpdateHereditament updateDto);
}
