namespace Common.Rest.Address.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
public class AddressController(IAddressService addressService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AddressDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AddressDocumentDto>>> CreateAddress(
        [FromBody] CreateUpdateAddress createDto,
        CancellationToken ct = default)
    {
        var result = await addressService.CreateAddressAsync(createDto, User?.FindFirst("sub")?.Value);
        var response = ApiResponse<AddressDocumentDto>.Ok(result, this.GetCorrelationId());
        return CreatedAtAction(nameof(GetAllAddresses), response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AddressDocumentDto>>> GetAddressById(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await addressService.GetAddressByIdAsync(id);
        if (result is null)
            throw new NotFoundException(nameof(AddressDocumentDto), id);

        return Ok(ApiResponse<AddressDocumentDto>.Ok(result, this.GetCorrelationId()));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<AddressDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<AddressDocumentDto>>> GetAllAddresses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await addressService.GetAllAddressesAsync(page, pageSize);
        var pagedResponse = new PagedApiResponse<AddressDocumentDto>(result.Data, page, pageSize, result.TotalCount, this.GetCorrelationId());
        return Ok(pagedResponse);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AddressDocumentDto>>> UpdateAddress(
        [FromRoute] Guid id,
        [FromBody] CreateUpdateAddress updateDto,
        CancellationToken ct = default)
    {
        var result = await addressService.UpdateAddressAsync(id, updateDto, User?.FindFirst("sub")?.Value);
        if (result is null)
            throw new NotFoundException(nameof(AddressDocumentDto), id);

        return Ok(ApiResponse<AddressDocumentDto>.Ok(result, this.GetCorrelationId()));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await addressService.DeleteAddressAsync(id, User?.FindFirst("sub")?.Value);
        if (!result)
            throw new NotFoundException(nameof(AddressDocumentDto), id);

        return NoContent();
    }

    [HttpDelete("{id:guid}/permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PermanentlyDeleteAddress(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await addressService.PermanentlyDeleteAddressAsync(id);
        if (!result)
            throw new NotFoundException(nameof(AddressDocumentDto), id);

        return NoContent();
    }

    [HttpGet("search/uprn")]
    [ProducesResponseType(typeof(ApiResponse<AddressDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AddressDocumentDto>>> SearchByUprn(
        [FromQuery] string uprn,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(uprn))
            throw new ValidationException("UPRN is required.");

        var result = await addressService.GetAddressByUprnAsync(uprn);
        if (result is null)
            throw new NotFoundException("Address", uprn);

        return Ok(ApiResponse<AddressDocumentDto>.Ok(result, this.GetCorrelationId()));
    }

    [HttpGet("search/advanced")]
    [ProducesResponseType(typeof(PagedApiResponse<AddressDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<AddressDocumentDto>>> AdvancedSearch(
        [FromQuery] string? postcode = null,
        [FromQuery] string? postTown = null,
        [FromQuery] string? organisation = null,
        [FromQuery] string? thoroughfare = null,
        [FromQuery] string? locality = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await addressService.AdvancedSearchAsync(
            postcode: postcode,
            postTown: postTown,
            organisation: organisation,
            thoroughfare: thoroughfare,
            locality: locality,
            page: page,
            pageSize: pageSize);

        var pagedResponse = new PagedApiResponse<AddressDocumentDto>(result.Data, page, pageSize, result.TotalCount, this.GetCorrelationId());
        return Ok(pagedResponse);
    }

    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetAddressCount(CancellationToken ct = default)
    {
        var result = await addressService.GetAddressCountAsync();
        return Ok(ApiResponse<int>.Ok(result, this.GetCorrelationId()));
    }

    [HttpPost("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAddress(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await addressService.RestoreAddressAsync(id, User?.FindFirst("sub")?.Value);
        if (!result)
            throw new NotFoundException(nameof(AddressDocumentDto), id);

        return NoContent();
    }
}
