namespace Common.Rest.SurveyData.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class SurveysController(ISurveyService surveyService) : ControllerBase
{
    /// <summary>
    /// Get a survey by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SurveyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await surveyService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<SurveyDto>.Ok(result, GetCorrelationId()));
    }

    /// <summary>
    /// Get a survey by its reference number.
    /// </summary>
    [HttpGet("by-reference/{referenceNumber}")]
    [ProducesResponseType(typeof(ApiResponse<SurveyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByReference(string referenceNumber, CancellationToken ct)
    {
        var result = await surveyService.GetByReferenceAsync(referenceNumber, ct);
        return Ok(ApiResponse<SurveyDto>.Ok(result, GetCorrelationId()));
    }

    /// <summary>
    /// Search surveys with filtering and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<SurveyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] SurveySearchRequest request, CancellationToken ct)
    {
        var (items, totalCount) = await surveyService.SearchAsync(request, ct);

        var response = new PagedApiResponse<SurveyDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            CorrelationId = GetCorrelationId()
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new survey.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SurveyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSurveyRequest request, CancellationToken ct)
    {
        var userId = User.Identity?.Name;
        var result = await surveyService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SurveyDto>.Ok(result, GetCorrelationId()));
    }

    /// <summary>
    /// Update an existing survey (partial update).
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SurveyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSurveyRequest request, CancellationToken ct)
    {
        var userId = User.Identity?.Name;
        var result = await surveyService.UpdateAsync(id, request, userId, ct);
        return Ok(ApiResponse<SurveyDto>.Ok(result, GetCorrelationId()));
    }

    /// <summary>
    /// Soft-delete a survey by its identifier.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await surveyService.DeleteAsync(id, ct);
        return NoContent();
    }

    private string? GetCorrelationId()
        => HttpContext.Items["CorrelationId"]?.ToString();
}
