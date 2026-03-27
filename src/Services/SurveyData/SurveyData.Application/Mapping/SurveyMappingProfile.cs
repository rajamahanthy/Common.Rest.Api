namespace SurveyData.Application.Mapping;

public class SurveyMappingProfile : Profile
{
    public SurveyMappingProfile()
    {
        CreateMap<Survey, SurveyDto>();
        CreateMap<SurveyDetail, SurveyDetailDto>();
        
        CreateMap<CreateSurveyRequest, Survey>();
        CreateMap<CreateSurveyDetailRequest, SurveyDetail>();
        
        CreateMap<UpdateSurveyRequest, Survey>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
