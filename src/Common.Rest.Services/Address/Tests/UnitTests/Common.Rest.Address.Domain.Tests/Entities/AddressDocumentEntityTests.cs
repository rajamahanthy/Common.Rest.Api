namespace Common.Rest.Address.Domain.Tests.Entities;

/// <summary>
/// Comprehensive unit tests for DocumentEntity<AddressEntity>.
/// Tests entity initialization, soft delete support, and update tracking.
/// </summary>
[TestClass]
public class AddressEntityTests
{
    private Guid _testId;

    [TestInitialize]
    public void Setup()
    {
        _testId = Guid.NewGuid();
    }

    private static AddressEntity CreateTestAddressEntity(string uprn = "123456789")
    {
        return new AddressEntity
        {
            Uprn = uprn,
            Usrn = "999999999",
            AddressInfo = new AddressInfoEntity
            {
                Organisation = "Test Org",
                Pao = new AddressableObjectEntity { StartNumber = 1, Text = "1" },
                StreetDescriptor = new StreetDescriptorEntity { StreetDescription = "Main St", PostTown = "Test Town" },
                Postcode = "T1 1ST"
            },
            Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
        };
    }

    #region Entity Initialization

    [TestMethod]
    public void Constructor_CreatesNewEntity()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.AreEqual(_testId, entity.Id);
        Assert.AreEqual("Address", entity.DocumentType);
        Assert.IsNotNull(entity.JsonData);
        Assert.IsFalse(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_PropertiesCanBeSet()
    {
        var createdTime = DateTimeOffset.UtcNow;
        var entity = new DocumentEntity<AddressEntity>
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            PartitionKey = "T1 1ST"
        };

        Assert.AreEqual("T1 1ST", entity.PartitionKey);
        Assert.AreEqual(createdTime, entity.CreatedAt);
        Assert.AreEqual("test-user", entity.CreatedBy);
    }

    #endregion

    #region Soft Delete Support

    [TestMethod]
    public void Entity_MarkAsDeleted_SetsIsDeletedFlag()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            IsDeleted = false
        };

        entity.IsDeleted = true;

        Assert.IsTrue(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_RestoreDeleted_ClearsIsDeletedFlag()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            IsDeleted = true
        };

        entity.IsDeleted = false;

        Assert.IsFalse(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_IsDeletedFalseByDefault()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.IsFalse(entity.IsDeleted);
    }

    #endregion

    #region Update Tracking

    [TestMethod]
    public void Entity_CreatedTimestamp_RecordsCreationTime()
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = now,
            CreatedBy = "creator-user"
        };

        Assert.AreEqual(now, entity.CreatedAt);
        Assert.AreEqual("creator-user", entity.CreatedBy);
    }

    [TestMethod]
    public void Entity_UpdatedTimestamp_TrackModifications()
    {
        var createdTime = DateTimeOffset.UtcNow.AddHours(-1);
        var updatedTime = DateTimeOffset.UtcNow;

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = createdTime,
            CreatedBy = "creator",
            UpdatedAt = updatedTime,
            UpdatedBy = "updater"
        };

        Assert.AreEqual(updatedTime, entity.UpdatedAt);
        Assert.AreEqual("updater", entity.UpdatedBy);
        Assert.IsTrue(entity.UpdatedAt > entity.CreatedAt);
    }

    [TestMethod]
    public void Entity_UpdatedPropertiesNullWhenNotModified()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.IsNull(entity.UpdatedAt);
        Assert.IsNull(entity.UpdatedBy);
    }

    [TestMethod]
    public void Entity_CanUpdateMultipleTimes()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        var firstUpdate = DateTimeOffset.UtcNow;
        entity.UpdatedAt = firstUpdate;
        entity.UpdatedBy = "user1";

        var secondUpdate = DateTimeOffset.UtcNow.AddMinutes(1);
        entity.UpdatedAt = secondUpdate;
        entity.UpdatedBy = "user2";

        Assert.AreEqual("user2", entity.UpdatedBy);
        Assert.AreEqual(secondUpdate, entity.UpdatedAt);
    }

    #endregion

    #region Document Type

    [TestMethod]
    public void Entity_DocumentType_CanBeSet()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.AreEqual("Address", entity.DocumentType);
    }

    #endregion

    #region JSON Data

    [TestMethod]
    public void Entity_JsonData_StoresAddressEntity()
    {
        var addressData = CreateTestAddressEntity();
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = addressData
        };

        Assert.AreEqual(addressData, entity.JsonData);
        Assert.AreEqual("123456789", entity.JsonData.Uprn);
    }

    [TestMethod]
    public void Entity_JsonData_CanBeUpdated()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        var updatedData = CreateTestAddressEntity("987654321");
        entity.JsonData = updatedData;

        Assert.AreEqual("987654321", entity.JsonData.Uprn);
    }

    #endregion

    #region Partition Key

    [TestMethod]
    public void Entity_PartitionKey_CanBeSet()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            PartitionKey = "T1 1ST"
        };

        Assert.AreEqual("T1 1ST", entity.PartitionKey);
    }

    [TestMethod]
    public void Entity_PartitionKey_IsRequired()
    {
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.IsNull(entity.PartitionKey);
    }

    #endregion

    #region Concurrency

    [TestMethod]
    public void Entity_RowVersion_TracksConcurrency()
    {
        var rowVersion = new byte[] { 0x00, 0x00, 0x00, 0x01 };
        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            RowVersion = rowVersion
        };

        Assert.AreEqual(rowVersion, entity.RowVersion);
    }

    #endregion
}
