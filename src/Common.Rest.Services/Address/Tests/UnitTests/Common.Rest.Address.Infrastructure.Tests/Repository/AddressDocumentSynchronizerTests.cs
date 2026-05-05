namespace Common.Rest.Address.Infrastructure.Tests.Repository;

using Common.Rest.Address.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

/// <summary>
/// Unit tests for AddressDocumentSynchronizer to verify denormalized field synchronization.
/// </summary>
[TestClass]
public class AddressDocumentSynchronizerTests
{
    private AddressDocumentSynchronizer _synchronizer = null!;

    [TestInitialize]
    public void Setup()
    {
        _synchronizer = new AddressDocumentSynchronizer();
    }

    private static AddressDocumentEntity CreateEntity(Guid? id = null)
    {
        return new AddressDocumentEntity
        {
            Id = id ?? Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                Usrn = "999999999",
                AddressInfo = new AddressInfoEntity
                {
                    Organisation = "Test Org",
                    Pao = new AddressableObjectEntity { StartNumber = 1, Text = "1" },
                    StreetDescriptor = new StreetDescriptorEntity 
                    { 
                        StreetDescription = "Main St", 
                        PostTown = "Test Town",
                        Locality = "Test Locality",
                        DependentLocality = "Test Dependent"
                    },
                    Postcode = "T1 1ST"
                },
                Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
            },
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };
    }

    [TestMethod]
    public void Synchronize_ValidEntity_PopulatesAllIndexes()
    {
        var entity = CreateEntity();

        _synchronizer.Synchronize(entity);

        Assert.AreEqual("123456789", entity.UprnIndex);
        Assert.AreEqual("T1 1ST", entity.PostcodeIndex);
        Assert.AreEqual("T1 1ST", entity.PartitionKey);
        Assert.AreEqual("Test Town", entity.PostTownIndex);
        Assert.AreEqual("Test Org", entity.OrganisationIndex);
        Assert.AreEqual("Main St", entity.ThoroughfareIndex);
        Assert.AreEqual("Test Locality", entity.LocalityIndex);
        Assert.AreEqual("Test Dependent", entity.DependentLocalityIndex);
    }

    [TestMethod]
    public void Synchronize_NullJsonData_DoesNotThrow()
    {
        var entity = CreateEntity();
        entity.JsonData = null;

        // Should not throw
        _synchronizer.Synchronize(entity);

        Assert.IsNull(entity.UprnIndex);
    }

    [TestMethod]
    public void Synchronize_PartialData_SynchronizesAvailableFields()
    {
        var entity = CreateEntity();
        entity.JsonData!.AddressInfo.Organisation = null;

        _synchronizer.Synchronize(entity);

        Assert.AreEqual("123456789", entity.UprnIndex);
        Assert.AreEqual("T1 1ST", entity.PostcodeIndex);
        Assert.IsNull(entity.OrganisationIndex);
    }

    [TestMethod]
    public void Synchronize_MissingPostcode_SetsPartitionKeyEmpty()
    {
        var entity = CreateEntity();
        entity.JsonData!.AddressInfo.Postcode = string.Empty;

        _synchronizer.Synchronize(entity);

        Assert.AreEqual(string.Empty, entity.PartitionKey);
    }

    [TestMethod]
    public void Synchronize_AllFieldsNull_NoThrow()
    {
        var entity = CreateEntity();
        entity.JsonData = new AddressEntity
        {
            Uprn = string.Empty,
            AddressInfo = new AddressInfoEntity
            {
                Pao = new AddressableObjectEntity { Text = string.Empty },
                StreetDescriptor = new StreetDescriptorEntity(),
                Postcode = string.Empty
            },
            Geography = new GeographyEntity()
        };

        // Should not throw
        _synchronizer.Synchronize(entity);

        Assert.IsNotNull(entity);
    }
}
