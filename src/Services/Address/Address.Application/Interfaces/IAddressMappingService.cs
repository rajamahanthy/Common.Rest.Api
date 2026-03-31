namespace Address.Application.Interfaces;

public interface IAddressMappingService
{
    AddressDto MapToAddressDto(AddressEntity address);
    AddressEntity MapToAddress(CreateAddressRequest request);
    void UpdateAddressFromRequest(AddressEntity address, UpdateAddressRequest request);
}
