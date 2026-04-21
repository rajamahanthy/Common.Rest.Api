namespace Common.Rest.Hereditament.Application.Dtos;

public record HereditamentDocumentDto(Guid Id, HereditamentDto HereditamentDetails);


[NoAdditionalProperties]
public class HereditamentDto
{
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; } = string.Empty;

    [JsonPropertyName("effectiveFrom")]    
    public DateOnly EffectiveFrom { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    
    [JsonPropertyName("addressId")]    
    public Guid? AddressId { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}

//public struct HereditamentStatus
//{
//    public const string Draft = "Draft";
//    public const string Active = "Active";
//    public const string Removed = "Removed";
//}

public class CreateUpdateHereditament
{
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; } = string.Empty;

    [JsonPropertyName("effectiveFrom")]
    [Required]
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [JsonPropertyName("addressId")]
    public Guid? AddressId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}