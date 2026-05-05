namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Address.Domain.Entities;

/// <summary>
/// Helper service to synchronize denormalized index fields with JsonData content.
/// Ensures PartitionKey (postcode) and search indexes are always in sync with the source JSON.
/// </summary>
public interface IAddressDocumentSynchronizer
{
    /// <summary>
    /// Populates denormalized fields from JsonData.
    /// </summary>
    void Synchronize(AddressDocumentEntity document);
}

/// <summary>
/// Implementation of address document synchronizer.
/// </summary>
public class AddressDocumentSynchronizer : IAddressDocumentSynchronizer
{
    public void Synchronize(AddressDocumentEntity document)
    {
        if (document?.JsonData == null)
            return;

        var json = document.JsonData;

        // Extract and denormalize UPRN
        document.UprnIndex = json.Uprn;

        // Extract and denormalize postcode (also set as partition key)
        document.PostcodeIndex = json.AddressInfo?.Postcode;
        document.PartitionKey = json.AddressInfo?.Postcode;

        // Extract and denormalize PostTown
        document.PostTownIndex = json.AddressInfo?.StreetDescriptor?.PostTown;

        // Extract and denormalize Organisation
        document.OrganisationIndex = json.AddressInfo?.Organisation;

        // Extract and denormalize StreetDescription (Thoroughfare)
        document.ThoroughfareIndex = json.AddressInfo?.StreetDescriptor?.StreetDescription;

        // Extract and denormalize Locality
        document.LocalityIndex = json.AddressInfo?.StreetDescriptor?.Locality;

        // Extract and denormalize DependentLocality
        document.DependentLocalityIndex = json.AddressInfo?.StreetDescriptor?.DependentLocality;
    }
}
