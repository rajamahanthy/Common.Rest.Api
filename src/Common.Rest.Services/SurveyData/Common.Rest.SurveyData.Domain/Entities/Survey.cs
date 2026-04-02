namespace Common.Rest.SurveyData.Domain.Entities;

/// <summary>
/// Represents a survey data record with property valuation information.
/// </summary>
public class Survey : BaseEntity
{
    private SurveyDetailCollection _details = null!;

    public Survey()
    {
        _details = new SurveyDetailCollection(this);
    }

    public string ReferenceNumber { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string? PostCode { get; set; }
    public string? LocalAuthority { get; set; }
    public string SurveyType { get; set; } = string.Empty;
    public DateTimeOffset SurveyDate { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Surveyor { get; set; }
    public string? Notes { get; set; }
    public decimal? AssessedValue { get; set; }
    public decimal? FloorArea { get; set; }
    public string? FloorAreaUnit { get; set; }
    public string? PropertyType { get; set; }
    public string? PropertySubType { get; set; }
    public SurveyInfo? SurveyJson { get; set; }

    // Navigation
    public ICollection<SurveyDetail> Details => _details;
}
