using Azure.Core;

namespace Common.Rest.SurveyData.Application.Services;

public class SurveyMappingService : ISurveyMappingService
{
    public SurveyDto MapToSurveyDto(Survey survey)
    {
        return new SurveyDto
        {
            Id = survey.Id,
            ReferenceNumber = survey.ReferenceNumber,
            PropertyAddress = survey.PropertyAddress,
            PostCode = survey.PostCode,
            LocalAuthority = survey.LocalAuthority,
            SurveyType = survey.SurveyType,
            SurveyDate = survey.SurveyDate,
            Status = survey.Status,
            Surveyor = survey.Surveyor,
            Notes = survey.Notes,
            AssessedValue = survey.AssessedValue,
            FloorArea = survey.FloorArea,
            FloorAreaUnit = survey.FloorAreaUnit,
            PropertyType = survey.PropertyType,
            PropertySubType = survey.PropertySubType,
            CreatedAt = survey.CreatedAt,
            SurveyInfo = survey.SurveyJson is null ? null : MapToSurveyInfoDto(survey.SurveyJson),
            Details = survey.Details.Select(MapToSurveyDetailDto).ToList()
        };
    }

    public Survey MapToSurvey(CreateSurveyRequest request)
    {
        var survey = new Survey
        {
            ReferenceNumber = request.ReferenceNumber,
            PropertyAddress = request.PropertyAddress,
            PostCode = request.PostCode,
            LocalAuthority = request.LocalAuthority,
            SurveyType = request.SurveyType,
            SurveyDate = request.SurveyDate ?? DateTimeOffset.UtcNow,
            Surveyor = request.Surveyor,
            Notes = request.Notes,
            AssessedValue = request.AssessedValue,
            FloorArea = request.FloorArea,
            FloorAreaUnit = request.FloorAreaUnit,
            PropertyType = request.PropertyType,
            PropertySubType = request.PropertySubType
        };

        if (request.Details is not null)
        {
            foreach (var detail in request.Details)
            {
                survey.Details.Add(MapToSurveyDetail(detail));
            }
        }

        return survey;
    }

    public SurveyDetail MapToSurveyDetail(CreateSurveyDetailRequest request)
    {
        return new SurveyDetail
        {
            Description = request.Description,
            Area = request.Area,
            AreaUnit = request.AreaUnit,
            RatePerUnit = request.RatePerUnit,
            Value = request.Value,
            SortOrder = request.SortOrder
        };
    }

    private static SurveyDetailDto MapToSurveyDetailDto(SurveyDetail detail)
    {
        return new SurveyDetailDto
        {
            Id = detail.Id,
            Description = detail.Description,
            Area = detail.Area,
            AreaUnit = detail.AreaUnit,
            RatePerUnit = detail.RatePerUnit,
            Value = detail.Value,
            SortOrder = detail.SortOrder
        };
    }

    private static SurveyInfoDto MapToSurveyInfoDto(SurveyInfo request)
    {
        return new SurveyInfoDto
        {
            ReferenceNumber = request.ReferenceNumber,
            PropertyAddress = request.PropertyAddress,
            PostCode = request.PostCode,
            LocalAuthority = request.LocalAuthority,
            SurveyType = request.SurveyType,
            SurveyDate = request.SurveyDate ?? DateTimeOffset.UtcNow,
            Surveyor = request.Surveyor,
            Notes = request.Notes,
            AssessedValue = request.AssessedValue,
            FloorArea = request.FloorArea,
            FloorAreaUnit = request.FloorAreaUnit,
            PropertyType = request.PropertyType,
            PropertySubType = request.PropertySubType
        };
    }
}
