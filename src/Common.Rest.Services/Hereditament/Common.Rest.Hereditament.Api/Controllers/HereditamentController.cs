namespace Common.Rest.Hereditament.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
public class HereditamentController(IHereditamentService HereditamentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HereditamentDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<HereditamentDocumentDto>>> CreateHereditament(
        [FromBody] CreateUpdateHereditament createDto,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.CreateHereditamentAsync(createDto, User?.FindFirst("sub")?.Value, ct);
        var response = ApiResponse<HereditamentDocumentDto>.Ok(result, this.GetCorrelationId());
        return CreatedAtAction(nameof(GetAllHereditamentes), response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HereditamentDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<HereditamentDocumentDto>>> GetHereditamentById(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.GetHereditamentByIdAsync(id, ct);
        if (result is null)
            throw new NotFoundException(nameof(HereditamentDocumentDto), id);

        return Ok(ApiResponse<HereditamentDocumentDto>.Ok(result, this.GetCorrelationId()));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<HereditamentDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<HereditamentDocumentDto>>> GetAllHereditamentes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.GetAllHereditamentesAsync(page, pageSize, ct);
        var pagedResponse = new PagedApiResponse<HereditamentDocumentDto>(result.Data, page, pageSize, result.TotalCount, this.GetCorrelationId());
        return Ok(pagedResponse);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HereditamentDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<HereditamentDocumentDto>>> UpdateHereditament(
        [FromRoute] Guid id,
        [FromBody] CreateUpdateHereditament updateDto,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.UpdateHereditamentAsync(id, updateDto, User?.FindFirst("sub")?.Value, ct);
        if (result is null)
            throw new NotFoundException(nameof(HereditamentDocumentDto), id);

        return Ok(ApiResponse<HereditamentDocumentDto>.Ok(result, this.GetCorrelationId()));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHereditament(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.DeleteHereditamentAsync(id, User?.FindFirst("sub")?.Value, ct);
        if (!result)
            throw new NotFoundException(nameof(HereditamentDocumentDto), id);

        return NoContent();
    }

    [HttpDelete("{id:guid}/permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PermanentlyDeleteHereditament(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.PermanentlyDeleteHereditamentAsync(id, ct);
        if (!result)
            throw new NotFoundException(nameof(HereditamentDocumentDto), id);

        return NoContent();
    }

    [HttpGet("search/advanced")]
    [ProducesResponseType(typeof(PagedApiResponse<HereditamentDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<HereditamentDocumentDto>>> AdvancedSearch(
        [FromQuery] string? name = null,
        [FromQuery] string? status = null,
        [FromQuery] DateOnly? effectiveFrom = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.AdvancedSearchAsync(
            name: name,
            status: status,
            effectiveFrom: effectiveFrom,
            page: page,
            pageSize: pageSize,
            ct: ct);

        var pagedResponse = new PagedApiResponse<HereditamentDocumentDto>(result.Data, page, pageSize, result.TotalCount, this.GetCorrelationId());
        return Ok(pagedResponse);
    }

    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetHereditamentCount(CancellationToken ct = default)
    {
        var result = await HereditamentService.GetHereditamentCountAsync(ct);
        return Ok(ApiResponse<int>.Ok(result, this.GetCorrelationId()));
    }

    [HttpPost("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreHereditament(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await HereditamentService.RestoreHereditamentAsync(id, User?.FindFirst("sub")?.Value, ct);
        if (!result)
            throw new NotFoundException(nameof(HereditamentDocumentDto), id);

        return NoContent();
    }
}
