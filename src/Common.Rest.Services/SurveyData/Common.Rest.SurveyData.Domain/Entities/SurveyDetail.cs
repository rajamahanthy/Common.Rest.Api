using Common.Rest.Shared.Domain;

namespace Common.Rest.SurveyData.Domain.Entities;

/// <summary>
/// Represents a detailed line item within a survey (e.g., floor or area breakdown).
/// </summary>
public class SurveyDetail : BaseEntity
{
    public Guid SurveyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public decimal? Area { get; set; }
    public string? AreaUnit { get; set; }
    public decimal? RatePerUnit { get; set; }
    public decimal? Value { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public Survey Survey { get; set; } = null!;
}
