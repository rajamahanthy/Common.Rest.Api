namespace Common.Rest.Address.Application.Services;

/// <summary>
/// Implementation of mapping service for Address entities.
/// Centralizes conversion logic between domain entities and DTOs.
/// </summary>
public class AddressMappingService : IAddressMappingService
{
    public AddressDocumentDto MapToDto(AddressDocumentEntity document)
    {
        ArgumentNullException.ThrowIfNull(document);
        
        var address = document.JsonData;
        ArgumentNullException.ThrowIfNull(address);  // Keep this (JsonData could be null)
        ArgumentNullException.ThrowIfNull(address.AddressInfo);  // Keep this

        var addressDto = new AddressDto
        {
            Uprn = address.Uprn,
            AddressInfo = new AddressInfo
            {
                Organisation = address.AddressInfo.Organisation,
                Department = address.AddressInfo.Department,
                Sao = address.AddressInfo.Sao != null ? new AddressableObject
                {
                    Text = address.AddressInfo.Sao.Text,
                    StartNumber = address.AddressInfo.Sao.StartNumber,
                    StartSuffix = address.AddressInfo.Sao.StartSuffix,
                    EndNumber = address.AddressInfo.Sao.EndNumber,
                    EndSuffix = address.AddressInfo.Sao.EndSuffix
                } : null,
                Pao = new AddressableObject
                {
                    Text = address.AddressInfo.Pao.Text,
                    StartNumber = address.AddressInfo.Pao.StartNumber,
                    StartSuffix = address.AddressInfo.Pao.StartSuffix,
                    EndNumber = address.AddressInfo.Pao.EndNumber,
                    EndSuffix = address.AddressInfo.Pao.EndSuffix
                },
                StreetDescriptor = new StreetDescriptor
                {
                    StreetDescription = address.AddressInfo.StreetDescriptor.StreetDescription,
                    Locality = address.AddressInfo.StreetDescriptor.Locality,
                    DependentLocality = address.AddressInfo.StreetDescriptor.DependentLocality,
                    DoubleDependentLocality = address.AddressInfo.StreetDescriptor.DoubleDependentLocality,
                    TownName = address.AddressInfo.StreetDescriptor.TownName,
                    PostTown = address.AddressInfo.StreetDescriptor.PostTown,
                    AdministrativeArea = address.AddressInfo.StreetDescriptor.AdministrativeArea
                },
                Postcode = address.AddressInfo.Postcode
            },
            Geography = new Geography
            {
                Easting = address.Geography.Easting,
                Northing = address.Geography.Northing
            }
        };

        return new AddressDocumentDto(document.Id, addressDto);
    }

    public AddressDocumentEntity MapToDomain(CreateUpdateAddress createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto);
        ArgumentNullException.ThrowIfNull(createDto.AddressInfo);
        ArgumentNullException.ThrowIfNull(createDto.AddressInfo.Pao);
        ArgumentNullException.ThrowIfNull(createDto.AddressInfo.StreetDescriptor);
        ArgumentNullException.ThrowIfNull(createDto.Geography);

        var uprn = Guid.NewGuid().ToString("N")[..12];

        var addressEntity = new AddressEntity
        {
            Uprn = uprn,
            AddressInfo = new AddressInfoEntity
            {
                Organisation = createDto.AddressInfo.Organisation,
                Department = createDto.AddressInfo.Department,
                Sao = createDto.AddressInfo.Sao != null ? new AddressableObjectEntity
                {
                    Text = createDto.AddressInfo.Sao.Text ?? string.Empty,
                    StartNumber = createDto.AddressInfo.Sao.StartNumber,
                    StartSuffix = createDto.AddressInfo.Sao.StartSuffix,
                    EndNumber = createDto.AddressInfo.Sao.EndNumber,
                    EndSuffix = createDto.AddressInfo.Sao.EndSuffix
                } : null,
                Pao = new AddressableObjectEntity
                {
                    Text = createDto.AddressInfo.Pao.Text ?? string.Empty,
                    StartNumber = createDto.AddressInfo.Pao.StartNumber,
                    StartSuffix = createDto.AddressInfo.Pao.StartSuffix,
                    EndNumber = createDto.AddressInfo.Pao.EndNumber,
                    EndSuffix = createDto.AddressInfo.Pao.EndSuffix
                },
                StreetDescriptor = new StreetDescriptorEntity
                {
                    StreetDescription = createDto.AddressInfo.StreetDescriptor.StreetDescription,
                    Locality = createDto.AddressInfo.StreetDescriptor.Locality,
                    DependentLocality = createDto.AddressInfo.StreetDescriptor.DependentLocality,
                    DoubleDependentLocality = createDto.AddressInfo.StreetDescriptor.DoubleDependentLocality,
                    TownName = createDto.AddressInfo.StreetDescriptor.TownName,
                    PostTown = createDto.AddressInfo.StreetDescriptor.PostTown,
                    AdministrativeArea = createDto.AddressInfo.StreetDescriptor.AdministrativeArea
                },
                Postcode = createDto.AddressInfo.Postcode
            },
            Geography = new GeographyEntity
            {
                Easting = createDto.Geography.Easting,
                Northing = createDto.Geography.Northing
            }
        };

        return new AddressDocumentEntity { DocumentType = "Address", JsonData = addressEntity };
    }

    public AddressDocumentEntity UpdateDomain(AddressDocumentEntity addressDocEntity, CreateUpdateAddress updateDto)
    {
        ArgumentNullException.ThrowIfNull(addressDocEntity);
        ArgumentNullException.ThrowIfNull(updateDto);
        ArgumentNullException.ThrowIfNull(addressDocEntity.JsonData);
        ArgumentNullException.ThrowIfNull(updateDto.AddressInfo);
        ArgumentNullException.ThrowIfNull(updateDto.AddressInfo.Pao);

        var existing = addressDocEntity.JsonData;

        var updatedEntity = new AddressEntity
        {
            Uprn = existing.Uprn,
            Usrn = existing.Usrn,
            AddressInfo = new AddressInfoEntity
            {
                Organisation = updateDto.AddressInfo.Organisation,
                Department = updateDto.AddressInfo.Department,
                Sao = updateDto.AddressInfo.Sao != null ? new AddressableObjectEntity
                {
                    Text = updateDto.AddressInfo.Sao.Text ?? string.Empty,
                    StartNumber = updateDto.AddressInfo.Sao.StartNumber,
                    StartSuffix = updateDto.AddressInfo.Sao.StartSuffix,
                    EndNumber = updateDto.AddressInfo.Sao.EndNumber,
                    EndSuffix = updateDto.AddressInfo.Sao.EndSuffix
                }: null,
                Pao = new AddressableObjectEntity
                {
                    Text = updateDto.AddressInfo.Pao.Text ?? string.Empty,
                    StartNumber = updateDto.AddressInfo.Pao.StartNumber,
                    StartSuffix = updateDto.AddressInfo.Pao.StartSuffix,
                    EndNumber = updateDto.AddressInfo.Pao.EndNumber,
                    EndSuffix = updateDto.AddressInfo.Pao.EndSuffix
                },
                StreetDescriptor = new StreetDescriptorEntity
                {
                    StreetDescription = updateDto.AddressInfo.StreetDescriptor.StreetDescription,
                    Locality = updateDto.AddressInfo.StreetDescriptor.Locality,
                    DependentLocality = updateDto.AddressInfo.StreetDescriptor.DependentLocality,
                    DoubleDependentLocality = updateDto.AddressInfo.StreetDescriptor.DoubleDependentLocality,
                    TownName = updateDto.AddressInfo.StreetDescriptor.TownName,
                    PostTown = updateDto.AddressInfo.StreetDescriptor.PostTown,
                    AdministrativeArea = updateDto.AddressInfo.StreetDescriptor.AdministrativeArea
                },
                Postcode = updateDto.AddressInfo.Postcode
            },
            Geography = updateDto.Geography != null ? new GeographyEntity
            {
                Easting = updateDto.Geography.Easting,
                Northing = updateDto.Geography.Northing
            } : existing.Geography
        };

        return new AddressDocumentEntity { DocumentType = "Address", JsonData = updatedEntity };
    }
}
