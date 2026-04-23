namespace Common.Rest.Address.Application.Dtos;

public record AddressDocumentDto(Guid Id, AddressDto AddressDetails);


public class AddressDto
{
    [JsonPropertyName("uprn")]
    [Required]
    [RegularExpression(@"^[1-9][0-9]{0,11}$")]
    public required string Uprn { get; set; } = string.Empty;

    [JsonPropertyName("usrn")]
    [RegularExpression(@"^[1-9][0-9]{0,11}$")]
    public string Usrn { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    [Required]
    public required AddressInfo AddressInfo { get; set; }

    [JsonPropertyName("geography")]
    [Required]
    public required Geography Geography { get; set; }    
}

[NoAdditionalProperties]
public class CreateUpdateAddress
{
    [JsonPropertyName("address")]
    [Required]
    public required AddressInfo AddressInfo { get; set; }

    [JsonPropertyName("geography")]
    [Required]
    public required Geography Geography { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}


[NoAdditionalProperties]
public class AddressInfo
{
    [JsonPropertyName("organisation")]
    [MaxLength(120)]
    public string Organisation { get; set; } = string.Empty;

    [JsonPropertyName("department")]
    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    [JsonPropertyName("sao")]
    public AddressableObject? Sao { get; set; }

    [JsonPropertyName("pao")]
    [Required]
    public required AddressableObject Pao { get; set; }

    [JsonPropertyName("streetDescriptor")]
    [Required]
    public required StreetDescriptor StreetDescriptor { get; set; }

    [JsonPropertyName("postcode")]
    [Required]
    [RegularExpression(@"^(GIR 0AA|(?:[A-PR-UWYZ][0-9][0-9A-HJKSTUW]?|[A-PR-UWYZ][A-HK-Y][0-9][0-9ABEHMNPRVWXY]?) [0-9][ABD-HJLNP-UW-Z]{2})$")]
    public required string Postcode { get; set; } = string.Empty;

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}



[NoAdditionalProperties]
public class StreetDescriptor
{
    [JsonPropertyName("streetDescription")]
    [Required]
    [MaxLength(120)]
    public string StreetDescription { get; set; } = string.Empty;

    [JsonPropertyName("locality")]
    [MaxLength(120)]
    public string Locality { get; set; } = string.Empty;

    [JsonPropertyName("dependentLocality")]
    [MaxLength(120)]
    public string DependentLocality { get; set; } = string.Empty;

    [JsonPropertyName("doubleDependentLocality")]
    [MaxLength(120)]
    public string DoubleDependentLocality { get; set; } = string.Empty;

    [JsonPropertyName("townName")]
    [MaxLength(120)]
    public string TownName { get; set; } = string.Empty;

    [JsonPropertyName("postTown")]
    [MaxLength(120)]
    public string PostTown { get; set; } = string.Empty;

    [JsonPropertyName("administrativeArea")]
    [MaxLength(120)]
    public string AdministrativeArea { get; set; } = string.Empty;

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}



[NoAdditionalProperties]
public class Geography
{
    [JsonPropertyName("easting")]
    [Required]
    [Range(0, int.MaxValue)]
    public int Easting { get; set; }

    [JsonPropertyName("northing")]
    [Required]
    [Range(0, int.MaxValue)]
    public int Northing { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}



[AnyOf(nameof(Text), nameof(StartNumber))]
[NoAdditionalProperties]
public class AddressableObject
{
    [JsonPropertyName("text")]
    [MaxLength(120)]
    public string? Text { get; set; }

    [JsonPropertyName("startNumber")]
    [Range(1, 99999)]
    public int? StartNumber { get; set; }

    [JsonPropertyName("startSuffix")]
    [RegularExpression(@"^[A-Za-z]$")]
    public string StartSuffix { get; set; } = string.Empty;

    [JsonPropertyName("endNumber")]
    [Range(1, 99999)]
    public int? EndNumber { get; set; }

    [JsonPropertyName("endSuffix")]
    [RegularExpression(@"^[A-Za-z]$")]
    public string EndSuffix { get; set; } = string.Empty;

    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData { get; set; }
}
