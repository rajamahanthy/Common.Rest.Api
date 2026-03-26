using RestApi.Shared.Domain;

namespace Address.Domain.Entities;

/// <summary>
/// Represents a standardized address record with geolocation details.
/// </summary>
public class Address : BaseEntity
{
    public string Uprn { get; set; } = default!;
    public string SingleLineAddress { get; set; } = default!;
    public string BuildingName { get; set; } = default!;
    public string BuildingNumber { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string Locality { get; set; } = default!;
    public string Town { get; set; } = default!;
    public string Postcode { get; set; } = default!;
    public string Country { get; set; } = "United Kingdom";

    // Latitude and Longitude for integration with mapping services
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>
    /// Additional address metadata stored as a JSON column in SQL.
    /// </summary>
    public AddressInfo? AddressJson { get; set; }
}
