namespace Address.Application.Services;

public class AddressMappingService : IAddressMappingService
{
    public AddressDto MapToAddressDto(AddressEntity address)
    {
        return new AddressDto(
            Id: address.Id,
            Uprn: address.Uprn,
            SingleLineAddress: address.SingleLineAddress,
            BuildingName: address.BuildingName,
            BuildingNumber: address.BuildingNumber,
            Street: address.Street,
            Locality: address.Locality,
            Town: address.Town,
            Postcode: address.Postcode,
            Country: address.Country,
            Latitude: address.Latitude,
            Longitude: address.Longitude,
            AdditionalInfo: address.AdditionalInfoJson is null ? null : MapToAdditionalInfoDto(address.AdditionalInfoJson),
            CreatedAt: address.CreatedAt
        );
    }

    public AddressEntity MapToAddress(CreateAddressRequest request)
    {
        return new AddressEntity
        {
            Uprn = request.Uprn,
            SingleLineAddress = request.SingleLineAddress,
            BuildingName = request.BuildingName,
            BuildingNumber = request.BuildingNumber,
            Street = request.Street,
            Locality = request.Locality,
            Town = request.Town,
            Postcode = request.Postcode,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            AdditionalInfoJson = request.AdditionalInfo is null ? null : MapToAdditionalInfo(request.AdditionalInfo)
        };
    }

    public void UpdateAddressFromRequest(AddressEntity address, UpdateAddressRequest request)
    {
        if (!string.IsNullOrEmpty(request.SingleLineAddress))
            address.SingleLineAddress = request.SingleLineAddress;
        
        if (!string.IsNullOrEmpty(request.BuildingName))
            address.BuildingName = request.BuildingName;
        
        if (!string.IsNullOrEmpty(request.BuildingNumber))
            address.BuildingNumber = request.BuildingNumber;
        
        if (!string.IsNullOrEmpty(request.Street))
            address.Street = request.Street;
        
        if (!string.IsNullOrEmpty(request.Locality))
            address.Locality = request.Locality;
        
        if (!string.IsNullOrEmpty(request.Town))
            address.Town = request.Town;
        
        if (!string.IsNullOrEmpty(request.Postcode))
            address.Postcode = request.Postcode;
        
        if (!string.IsNullOrEmpty(request.Country))
            address.Country = request.Country;
        
        if (request.Latitude.HasValue)
            address.Latitude = request.Latitude;
        
        if (request.Longitude.HasValue)
            address.Longitude = request.Longitude;
        
        if (request.AdditionalInfo is not null)
            address.AdditionalInfoJson = MapToAdditionalInfo(request.AdditionalInfo);
    }

    private static AdditionalInfoDto? MapToAdditionalInfoDto(Address.Domain.Entities.AdditionalInfo additionalInfo)
    {
        return new AdditionalInfoDto(
            AddressLine1: additionalInfo.AddressLine1,
            AddressLine2: additionalInfo.AddressLine2,
            AddressLine3: additionalInfo.AddressLine3,
            AddressLine4: additionalInfo.AddressLine4,
            AddressLine5: additionalInfo.AddressLine5
        );
    }

    private static Address.Domain.Entities.AdditionalInfo MapToAdditionalInfo(AdditionalInfoDto dto)
    {
        return new Address.Domain.Entities.AdditionalInfo
        {
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            AddressLine3 = dto.AddressLine3,
            AddressLine4 = dto.AddressLine4,
            AddressLine5 = dto.AddressLine5
        };
    }
}
