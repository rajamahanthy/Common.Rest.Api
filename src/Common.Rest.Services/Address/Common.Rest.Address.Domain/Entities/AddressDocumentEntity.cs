namespace Common.Rest.Address.Domain.Entities;

using Common.Rest.Shared.Domain;

/// <summary>
/// Represents an address document with computed columns for efficient JSON property querying.
/// </summary>
public class AddressDocumentEntity : DocumentEntity<AddressEntity>
{
    /// <summary>
    /// Computed column: Extracted UPRN from JsonData for efficient querying.
    /// Maps to: $.uprn
    /// </summary>
    public string? UprnIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted Postcode from JsonData for efficient querying.
    /// Maps to: $.address.postcode
    /// </summary>
    public string? PostcodeIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted PostTown from JsonData for efficient querying.
    /// Maps to: $.address.streetDescriptor.postTown
    /// </summary>
    public string? PostTownIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted Organisation from JsonData for efficient querying.
    /// Maps to: $.address.organisation
    /// </summary>
    public string? OrganisationIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted Thoroughfare (StreetDescription) from JsonData for efficient querying.
    /// Maps to: $.address.streetDescriptor.streetDescription
    /// </summary>
    public string? ThoroughfareIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted Locality from JsonData for efficient querying.
    /// Maps to: $.address.streetDescriptor.locality
    /// </summary>
    public string? LocalityIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted DependentLocality from JsonData for efficient querying.
    /// Maps to: $.address.streetDescriptor.dependentLocality
    /// </summary>
    public string? DependentLocalityIndex { get; set; }
}
