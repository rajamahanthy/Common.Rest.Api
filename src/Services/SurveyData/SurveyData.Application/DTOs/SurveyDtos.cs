namespace SurveyData.Application.DTOs;

public sealed record SurveyDto
{
    public Guid Id { get; init; }
    public string ReferenceNumber { get; init; } = string.Empty;
    public string PropertyAddress { get; init; } = string.Empty;
    public string? PostCode { get; init; }
    public string? LocalAuthority { get; init; }
    public string SurveyType { get; init; } = string.Empty;
    public DateTimeOffset SurveyDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Surveyor { get; init; }
    public string? Notes { get; init; }
    public decimal? AssessedValue { get; init; }
    public decimal? FloorArea { get; init; }
    public string? FloorAreaUnit { get; init; }
    public string? PropertyType { get; init; }
    public string? PropertySubType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public SurveyInfoDto? SurveyInfo { get; init; }
    public IReadOnlyList<SurveyDetailDto> Details { get; init; } = [];
}

public sealed record SurveyDetailDto
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal? Area { get; init; }
    public string? AreaUnit { get; init; }
    public decimal? RatePerUnit { get; init; }
    public decimal? Value { get; init; }
    public int SortOrder { get; init; }
}

public sealed record SurveyInfoDto
{
    public string Uprn { get; init; } = string.Empty;
    public string SingleLineAddress { get; init; } = string.Empty;
    public string BuildingName { get; init; } = string.Empty;
    public string BuildingNumber { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Locality { get; init; } = string.Empty;
    public string Town { get; init; } = string.Empty;
    public string Postcode { get; init; } = string.Empty;
}

public sealed record CreateSurveyRequest
{
    public required string ReferenceNumber { get; init; }
    public required string PropertyAddress { get; init; }
    public string? PostCode { get; init; }
    public string? LocalAuthority { get; init; }
    public required string SurveyType { get; init; }
    public DateTimeOffset? SurveyDate { get; init; }
    public string? Surveyor { get; init; }
    public string? Notes { get; init; }
    public decimal? AssessedValue { get; init; }
    public decimal? FloorArea { get; init; }
    public string? FloorAreaUnit { get; init; }
    public string? PropertyType { get; init; }
    public string? PropertySubType { get; init; }
    public IReadOnlyList<CreateSurveyDetailRequest>? Details { get; init; }
}

public sealed record CreateSurveyDetailRequest
{
    public required string Description { get; init; }
    public decimal? Area { get; init; }
    public string? AreaUnit { get; init; }
    public decimal? RatePerUnit { get; init; }
    public decimal? Value { get; init; }
    public int SortOrder { get; init; }
}

public sealed record UpdateSurveyRequest
{
    public string? PropertyAddress { get; init; }
    public string? PostCode { get; init; }
    public string? LocalAuthority { get; init; }
    public string? SurveyType { get; init; }
    public DateTimeOffset? SurveyDate { get; init; }
    public string? Status { get; init; }
    public string? Surveyor { get; init; }
    public string? Notes { get; init; }
    public decimal? AssessedValue { get; init; }
    public decimal? FloorArea { get; init; }
    public string? FloorAreaUnit { get; init; }
    public string? PropertyType { get; init; }
    public string? PropertySubType { get; init; }
}

public sealed record SurveySearchRequest
{
    public string? ReferenceNumber { get; init; }
    public string? PostCode { get; init; }
    public string? LocalAuthority { get; init; }
    public string? SurveyType { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool Descending { get; init; }
}
