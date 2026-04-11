namespace Common.Rest.Address.Application.Dtos;

public record AddressDocumentDto(Guid Id, AddressDto AddressDetails);

public class AddressDto
{
    [JsonPropertyName("uprn")]
    public string Uprn { get; set; }

    [JsonPropertyName("address")]
    public AddressInfo AddressInfo { get; set; }

    [JsonPropertyName("geography")]
    public Geography Geography { get; set; }
}

public class CreateUpdateAddress
{
    [JsonPropertyName("address")]
    [Required]
    public AddressInfo AddressInfo { get; set; }

    [JsonPropertyName("geography")]
    [Required]
    public Geography Geography { get; set; }
}

public class AddressInfo
{
    [JsonPropertyName("organisation")]
    [MaxLength(120)]
    public string? Organisation { get; set; }

    [JsonPropertyName("department")]
    [MaxLength(120)]
    public string? Department { get; set; }

    [JsonPropertyName("sao")]
    public AddressableObject? Sao { get; set; }

    [JsonPropertyName("pao")]
    [Required]
    public AddressableObject Pao { get; set; }

    [JsonPropertyName("streetDescriptor")]
    [Required]
    public StreetDescriptor StreetDescriptor { get; set; }

    [JsonPropertyName("postcode")]
    [RegularExpression(@"^(GIR 0AA|([A-PR-UWYZ][0-9][0-9]?|[A-PR-UWYZ][A-HK-Y][0-9][0-9]?|[A-PR-UWYZ][0-9][A-HJKSTUW]|[A-PR-UWYZ][A-HK-Y][0-9][ABEHMNPRVWXY]) [0-9][ABD-HJLNP-UW-Z]{2})$")]
    public string? Postcode { get; set; }
}

public class StreetDescriptor
{
    [JsonPropertyName("streetDescription")]
    [Required]
    [MaxLength(120)]
    public string StreetDescription { get; set; }

    [JsonPropertyName("locality")]
    [MaxLength(120)]
    public string? Locality { get; set; }

    [JsonPropertyName("dependentLocality")]
    [MaxLength(120)]
    public string? DependentLocality { get; set; }

    [JsonPropertyName("doubleDependentLocality")]
    [MaxLength(120)]
    public string? DoubleDependentLocality { get; set; }

    [JsonPropertyName("townName")]
    [MaxLength(120)]
    public string? TownName { get; set; }

    [JsonPropertyName("postTown")]
    [MaxLength(120)]
    public string? PostTown { get; set; }

    [JsonPropertyName("administrativeArea")]
    [MaxLength(120)]
    public string? AdministrativeArea { get; set; }
}

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
}

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
    public string? StartSuffix { get; set; }

    [JsonPropertyName("endNumber")]
    [Range(1, 99999)]
    public int? EndNumber { get; set; }

    [JsonPropertyName("endSuffix")]
    [RegularExpression(@"^[A-Za-z]$")]
    public string? EndSuffix { get; set; }
}