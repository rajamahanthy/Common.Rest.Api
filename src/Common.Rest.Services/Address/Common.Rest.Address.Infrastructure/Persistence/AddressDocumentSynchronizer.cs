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
    void Synchronize(DocumentEntity<AddressEntity> document);
}

/// <summary>
/// Implementation of address document synchronizer.
/// </summary>
public class AddressDocumentSynchronizer : IAddressDocumentSynchronizer
{
    public void Synchronize(DocumentEntity<AddressEntity> document)
    {
        if (document?.JsonData == null)
            return;

        if (document?.JsonData?.AddressInfo?.Postcode == null)
            throw new ArgumentException("Postcode in JsonData.AddressInfo must be set before persisting to Cosmos.", nameof(document));
    }
}
