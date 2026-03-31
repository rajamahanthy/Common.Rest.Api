namespace SurveyData.Application.Interfaces;

public interface ISurveyMappingService
{
    SurveyDto MapToSurveyDto(Survey survey);
    Survey MapToSurvey(CreateSurveyRequest request);
    SurveyDetail MapToSurveyDetail(CreateSurveyDetailRequest request);
}
