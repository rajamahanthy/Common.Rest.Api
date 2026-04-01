namespace Common.Rest.Address.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // Standardized Auth (Mocked in Local Dev)
public class AddressesController(IAddressService addressService) : ControllerBase
{
    /// <summary>
    /// Search standardized address records.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<AddressDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await addressService.GetPagedAddressesAsync(page, pageSize, searchTerm, ct);
        return Ok(result);
    }

    /// <summary>
    /// Retrieve address by unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await addressService.GetAddressByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Register a new standardized address record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AddressDto>> Create([FromBody] CreateAddressRequest request, CancellationToken ct = default)
    {
        var result = await addressService.CreateAddressAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing address record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct = default)
    {
        await addressService.UpdateAddressAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Soft-delete an address record.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await addressService.DeleteAddressAsync(id, ct);
        return NoContent();
    }
}
