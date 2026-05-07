namespace Common.Rest.Address.Infrastructure.Tests.Repository;

using Common.Rest.Address.Domain.Entities;
using Common.Rest.Address.Infrastructure.Persistence;

[TestClass]
public class AddressDocumentSynchronizerTests
{
    private AddressDocumentSynchronizer _synchronizer = null!;

    [TestInitialize]
    public void Setup()
    {
        _synchronizer = new AddressDocumentSynchronizer();
    }

    private static AddressEntity CreateTestAddressEntity()
    {
        return new AddressEntity
        {
            Uprn = "123456789",
            Usrn = "999999999",
            AddressInfo = new AddressInfoEntity
            {
                Organisation = "Test Org",
                Pao = new AddressableObjectEntity { StartNumber = 1, Text = "1" },
                StreetDescriptor = new StreetDescriptorEntity { StreetDescription = "Main St", PostTown = "Test Town", Locality = "Test Locality", DependentLocality = "Test Dependent" },
                Postcode = "T1 1ST"
            },
            Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
        };
    }

    [TestMethod]
    public void Synchronize_SetsPartitionKeyFromPostcode()
    {
        var document = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        _synchronizer.Synchronize(document);

        Assert.AreEqual("T1 1ST", document.PartitionKey);
    }

    [TestMethod]
    public void Synchronize_WithNullJsonData_DoesNothing()
    {
        var document = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = null!
        };

        _synchronizer.Synchronize(document);

        Assert.IsNull(document.PartitionKey);
    }

    [TestMethod]
    public void Synchronize_WithNullAddressInfo_SetsPartitionKeyToNull()
    {
        var addressData = CreateTestAddressEntity();
        addressData.AddressInfo = null!;

        var document = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = addressData
        };

        _synchronizer.Synchronize(document);

        Assert.IsNull(document.PartitionKey);
    }

    [TestMethod]
    public void Synchronize_WithNullPostcode_SetsPartitionKeyToNull()
    {
        var addressData = CreateTestAddressEntity();
        addressData.AddressInfo.Postcode = null!;

        var document = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = addressData
        };

        _synchronizer.Synchronize(document);

        Assert.IsNull(document.PartitionKey);
    }
}
