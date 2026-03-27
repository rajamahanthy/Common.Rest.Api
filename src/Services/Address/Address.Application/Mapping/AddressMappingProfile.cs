namespace Address.Application.Mapping;

public class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<Address.Domain.Entities.Address, AddressDto>();
        CreateMap<CreateAddressRequest, Address.Domain.Entities.Address>();
        CreateMap<UpdateAddressRequest, Address.Domain.Entities.Address>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
