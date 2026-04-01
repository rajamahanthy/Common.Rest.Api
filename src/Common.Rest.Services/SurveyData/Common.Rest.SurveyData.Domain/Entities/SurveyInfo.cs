namespace Common.Rest.SurveyData.Domain.Entities;

/// <summary>
/// POCO representing the address details stored as a JSON blob in the database.
/// Used for SurveyJson column mapping in the Survey entity.
/// </summary>
public record SurveyInfo
{
    public string Uprn { get; set; } = string.Empty;
    public string SingleLineAddress { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
}
