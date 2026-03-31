namespace Address.Application.Mapping;

public class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<AdditionalInfo, AdditionalInfoDto>().ReverseMap();

        CreateMap<Address.Domain.Entities.Address, AddressDto>()
            .ForMember(d => d.AdditionalInfo, opt => opt.MapFrom(s => s.AdditionalInfoJson));

        CreateMap<CreateAddressRequest, Address.Domain.Entities.Address>()
            .ForMember(d => d.AdditionalInfoJson, opt => opt.MapFrom(s => s.AdditionalInfo));

        CreateMap<UpdateAddressRequest, Address.Domain.Entities.Address>()
            .ForMember(d => d.AdditionalInfoJson, opt => opt.MapFrom(s => s.AdditionalInfo))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
