namespace Address.Domain.Entities;

/// <summary>
/// POCO representing the address details stored as a JSON blob in the database.
/// </summary>
public record AdditionalInfo
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string AddressLine3 { get; set; } = string.Empty;
    public string AddressLine4 { get; set; } = string.Empty;
    public string AddressLine5 { get; set; } = string.Empty;
}
