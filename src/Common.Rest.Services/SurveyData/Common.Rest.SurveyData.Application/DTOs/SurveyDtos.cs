namespace Common.Rest.SurveyData.Application.Dtos;

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
    public string ReferenceNumber { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string? PostCode { get; set; }
    public string? LocalAuthority { get; set; }
    public string SurveyType { get; set; } = string.Empty;
    public DateTimeOffset SurveyDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Surveyor { get; set; }
    public string? Notes { get; set; }
    public decimal? AssessedValue { get; set; }
    public decimal? FloorArea { get; set; }
    public string? FloorAreaUnit { get; set; }
    public string? PropertyType { get; set; }
    public string? PropertySubType { get; set; }
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
