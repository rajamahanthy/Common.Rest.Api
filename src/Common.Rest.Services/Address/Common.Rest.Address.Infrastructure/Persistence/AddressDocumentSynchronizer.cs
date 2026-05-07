namespace Common.Rest.Address.Infrastructure.Persistence;

using Common.Rest.Address.Domain.Entities;

/// <summary>
/// Helper service to synchronize partition key with JsonData content.
/// Ensures PartitionKey (postcode) is always in sync with the source JSON.
/// </summary>
public interface IAddressDocumentSynchronizer
{
    /// <summary>
    /// Populates partition key from JsonData.
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

        // Set partition key from postcode
        document.PartitionKey = json.AddressInfo?.Postcode;
    }
}
