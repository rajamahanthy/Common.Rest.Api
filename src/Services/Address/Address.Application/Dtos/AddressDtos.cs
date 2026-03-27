namespace Address.Application.DTOs;

public record AddressDto(
    Guid Id,
    string Uprn,
    string SingleLineAddress,
    string BuildingName,
    string BuildingNumber,
    string Street,
    string Locality,
    string Town,
    string Postcode,
    string Country,
    double? Latitude,
    double? Longitude,
    DateTimeOffset CreatedAt
);

public record CreateAddressRequest(
    [Required] string Uprn,
    [Required] string SingleLineAddress,
    string BuildingName,
    string BuildingNumber,
    [Required] string Street,
    string Locality,
    [Required] string Town,
    [Required] string Postcode,
    string Country = "United Kingdom",
    double? Latitude = null,
    double? Longitude = null
);

public record UpdateAddressRequest(
    string SingleLineAddress,
    string BuildingName,
    string BuildingNumber,
    string Street,
    string Locality,
    string Town,
    string Postcode,
    string Country,
    double? Latitude,
    double? Longitude
);
