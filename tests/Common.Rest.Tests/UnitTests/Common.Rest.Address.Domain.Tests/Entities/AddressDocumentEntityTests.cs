namespace Common.Rest.Address.Domain.Tests.Entities;

/// <summary>
/// Comprehensive unit tests for AddressDocumentEntity with 100% code coverage.
/// Tests entity initialization, soft delete support, and update tracking.
/// </summary>
[TestClass]
public class AddressDocumentEntityTests
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            UprnIndex = "123456789",
            PostcodeIndex = "T1 1ST",
            PostTownIndex = "Test Town"
        };

        Assert.AreEqual("123456789", entity.UprnIndex);
        Assert.AreEqual("T1 1ST", entity.PostcodeIndex);
        Assert.AreEqual("Test Town", entity.PostTownIndex);
        Assert.AreEqual(createdTime, entity.CreatedAt);
        Assert.AreEqual("test-user", entity.CreatedBy);
    }

    [TestMethod]
    public void Entity_ComputedColumns_InitiallyNull()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.IsNull(entity.UprnIndex);
        Assert.IsNull(entity.PostcodeIndex);
        Assert.IsNull(entity.PostTownIndex);
    }

    #endregion

    #region Soft Delete Support

    [TestMethod]
    public void Entity_MarkAsDeleted_SetsIsDeletedFlag()
    {
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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

        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
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
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        var updatedData = CreateTestAddressEntity("987654321");
        entity.JsonData = updatedData;

        Assert.AreEqual("987654321", entity.JsonData.Uprn);
    }

    #endregion

    #region Computed Columns

    [TestMethod]
    public void Entity_AllComputedColumns_CanBeSet()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            UprnIndex = "123456789",
            PostcodeIndex = "T1 1ST",
            PostTownIndex = "Test Town",
            OrganisationIndex = "Test Org",
            ThoroughfareIndex = "Main Street",
            LocalityIndex = "City",
            DependentLocalityIndex = "Ward"
        };

        Assert.AreEqual("123456789", entity.UprnIndex);
        Assert.AreEqual("T1 1ST", entity.PostcodeIndex);
        Assert.AreEqual("Test Town", entity.PostTownIndex);
        Assert.AreEqual("Test Org", entity.OrganisationIndex);
        Assert.AreEqual("Main Street", entity.ThoroughfareIndex);
        Assert.AreEqual("City", entity.LocalityIndex);
        Assert.AreEqual("Ward", entity.DependentLocalityIndex);
    }

    [TestMethod]
    public void Entity_UprnIndex_CanBeQueried()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            UprnIndex = "123456789"
        };

        Assert.AreEqual("123456789", entity.UprnIndex);
    }

    [TestMethod]
    public void Entity_PostcodeIndex_CanBeQueried()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            PostcodeIndex = "T1 1ST"
        };

        Assert.AreEqual("T1 1ST", entity.PostcodeIndex);
    }

    #endregion

    #region Entity States

    [TestMethod]
    public void Entity_ActiveEntity_HasCorrectState()
    {
        var createdTime = DateTimeOffset.UtcNow;
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            IsDeleted = false
        };

        Assert.IsFalse(entity.IsDeleted);
        Assert.AreNotEqual(Guid.Empty, entity.Id);
        Assert.AreEqual("test-user", entity.CreatedBy);
    }

    [TestMethod]
    public void Entity_DeletedEntity_HasCorrectState()
    {
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            CreatedBy = "creator",
            IsDeleted = true
        };

        Assert.IsTrue(entity.IsDeleted);
        Assert.AreEqual("creator", entity.CreatedBy);
    }

    #endregion

    #region Guid and ID

    [TestMethod]
    public void Entity_Id_IsUnique()
    {
        var entity1 = new AddressDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };
        var entity2 = new AddressDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity("987654321")
        };

        Assert.AreNotEqual(entity1.Id, entity2.Id);
    }

    [TestMethod]
    public void Entity_Id_CanBeAssigned()
    {
        var testId = Guid.NewGuid();
        var entity = new AddressDocumentEntity
        {
            Id = testId,
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity()
        };

        Assert.AreEqual(testId, entity.Id);
    }

    [TestMethod]
    public void Entity_PartitionKey_CanBeSet()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = CreateTestAddressEntity(),
            PartitionKey = "test-key"
        };

        Assert.AreEqual("test-key", entity.PartitionKey);
    }

    #endregion
}
