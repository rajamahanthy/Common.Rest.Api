namespace Common.Rest.Address.Api.Tests.TestData;

/// <summary>
/// Generic test data builder for creating Address DTOs and related objects.
/// Minimizes duplication across test classes.
/// </summary>
public static class AddressTestDataBuilder
{
    public static AddressDocumentDto CreateAddressDocumentDto(Guid? id = null)
    {
        return new AddressDocumentDto(
            id ?? Guid.NewGuid(),
            CreateAddressDto()
        );
    }

    public static AddressDto CreateAddressDto()
    {
        return new AddressDto
        {
            Uprn = "123456789",
            AddressInfo = CreateAddressInfo(),
            Geography = CreateGeography()
        };
    }

    public static CreateUpdateAddress CreateUpdateAddressDto()
    {
        return new CreateUpdateAddress
        {
            AddressInfo = CreateAddressInfo(),
            Geography = CreateGeography()
        };
    }

    private static AddressInfo CreateAddressInfo()
    {
        return new AddressInfo
        {
            Organisation = "Test Organisation",
            Department = null,
            Pao = new AddressableObject { StartNumber = 123 },
            Sao = null,
            StreetDescriptor = new StreetDescriptor
            {
                StreetDescription = "Main Street",
                PostTown = "Test Town",
                Locality = "Test Locality",
                AdministrativeArea = "Test Region"
            },
            Postcode = "T1 1ST"
        };
    }

    private static Geography CreateGeography()
    {
        return new Geography { Easting = 123456, Northing = 654321 };
    }
}
