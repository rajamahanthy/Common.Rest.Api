namespace Address.Application.DTOs;

public record AdditionalInfoDto(
    string AddressLine1,
    string AddressLine2,
    string AddressLine3,
    string AddressLine4,
    string AddressLine5
);

public record AddressDto(
    Guid Id = default,
    string Uprn = "",
    string SingleLineAddress = "",
    string BuildingName = "",
    string BuildingNumber = "",
    string Street = "",
    string Locality = "",
    string Town = "",
    string Postcode = "",
    string Country = "",
    double? Latitude = null,
    double? Longitude = null,
    AdditionalInfoDto? AdditionalInfo = null,
    DateTimeOffset CreatedAt = default
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
    double? Longitude = null,
    AdditionalInfoDto? AdditionalInfo = null
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
    double? Longitude,
    AdditionalInfoDto? AdditionalInfo
);
