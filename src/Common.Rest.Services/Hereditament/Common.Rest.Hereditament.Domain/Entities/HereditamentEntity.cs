namespace Common.Rest.Hereditament.Domain.Entities;

using Common.Rest.Shared.CustomValidations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class HereditamentEntity
{
    [JsonPropertyName("uarn")]
    [Required]
    public required Guid UARN { get; set; } = Guid.NewGuid();
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; } = string.Empty;
    [JsonPropertyName("effectiveFrom")]
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    [JsonPropertyName("status")]
    [Required]
    public required string Status { get; set; } = HereditamentStatus.Draft;
    [JsonPropertyName("addressId")]
    public Guid? AddressId { get; set; }
}

public struct HereditamentStatus
{
    public const string Draft = "Draft";
    public const string Active = "Active";
    public const string Removed = "Removed";
}