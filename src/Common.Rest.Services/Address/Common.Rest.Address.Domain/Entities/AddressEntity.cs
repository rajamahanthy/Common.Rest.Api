namespace Common.Rest.Address.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class AddressEntity
{
    [JsonPropertyName("uprn")]
    [Required]
    [RegularExpression(@"^[1-9][0-9]{0,11}$")]
    public string Uprn { get; set; }

    [JsonPropertyName("usrn")]
    [RegularExpression(@"^[1-9][0-9]{0,11}$")]
    public string? Usrn { get; set; }

    [JsonPropertyName("address")]
    [Required]
    public AddressInfoEntity AddressInfo { get; set; }

    [JsonPropertyName("geography")]
    [Required]
    public GeographyEntity Geography { get; set; }
}

public class AddressInfoEntity
{
    [JsonPropertyName("organisation")]
    [MaxLength(120)]
    public string? Organisation { get; set; }

    [JsonPropertyName("department")]
    [MaxLength(120)]
    public string? Department { get; set; }

    [JsonPropertyName("sao")]
    public AddressableObjectEntity? Sao { get; set; }

    [JsonPropertyName("pao")]
    [Required]
    public AddressableObjectEntity Pao { get; set; }

    [JsonPropertyName("streetDescriptor")]
    [Required]
    public StreetDescriptorEntity StreetDescriptor { get; set; }

    [JsonPropertyName("postcode")]
    [RegularExpression(@"^(GIR 0AA|([A-PR-UWYZ][0-9][0-9]?|[A-PR-UWYZ][A-HK-Y][0-9][0-9]?|[A-PR-UWYZ][0-9][A-HJKSTUW]|[A-PR-UWYZ][A-HK-Y][0-9][ABEHMNPRVWXY]) [0-9][ABD-HJLNP-UW-Z]{2})$")]
    public string? Postcode { get; set; }
}

public class StreetDescriptorEntity
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

public class GeographyEntity
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

public class AddressableObjectEntity
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