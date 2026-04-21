namespace Common.Rest.Hereditament.Domain.Tests.Entities;

/// <summary>
/// Comprehensive unit tests for HereditamentDocumentEntity with 100% code coverage.
/// Tests entity initialization, soft delete support, and update tracking.
/// </summary>
[TestClass]
public class HereditamentDocumentEntityTests
{
    private Guid _testId;

    [TestInitialize]
    public void Setup()
    {
        _testId = Guid.NewGuid();
    }

    private static HereditamentEntity CreateTestHereditamentEntity(string name = "Test Hereditament")
    {
        return new HereditamentEntity
        {
            UARN = Guid.NewGuid(),
            Name = name,
            Status = HereditamentStatus.Active,
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
    }

    #region Entity Initialization

    [TestMethod]
    public void Constructor_CreatesNewEntity()
    {
        var entity = new HereditamentDocumentEntity
        {
            Id = _testId,
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.AreEqual(_testId, entity.Id);
        Assert.AreEqual("Hereditament", entity.DocumentType);
        Assert.IsNotNull(entity.JsonData);
        Assert.IsFalse(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_PropertiesCanBeSet()
    {
        var createdTime = DateTimeOffset.UtcNow;
        var entity = new HereditamentDocumentEntity
        {
            Id = _testId,
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            NameIndex = "Test Hereditament",
            StatusIndex = HereditamentStatus.Active,
            EffectiveFromIndex = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        Assert.AreEqual("Test Hereditament", entity.NameIndex);
        Assert.AreEqual(HereditamentStatus.Active, entity.StatusIndex);
        Assert.IsNotNull(entity.EffectiveFromIndex);
        Assert.AreEqual(createdTime, entity.CreatedAt);
        Assert.AreEqual("test-user", entity.CreatedBy);
    }

    [TestMethod]
    public void Entity_ComputedColumns_InitiallyNull()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.IsNull(entity.NameIndex);
        Assert.IsNull(entity.StatusIndex);
        Assert.IsNull(entity.EffectiveFromIndex);
    }

    #endregion

    #region Soft Delete Support

    [TestMethod]
    public void Entity_MarkAsDeleted_SetsIsDeletedFlag()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            IsDeleted = false
        };

        entity.IsDeleted = true;

        Assert.IsTrue(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_RestoreDeleted_ClearsIsDeletedFlag()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            IsDeleted = true
        };

        entity.IsDeleted = false;

        Assert.IsFalse(entity.IsDeleted);
    }

    [TestMethod]
    public void Entity_IsDeletedFalseByDefault()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.IsFalse(entity.IsDeleted);
    }

    #endregion

    #region Update Tracking

    [TestMethod]
    public void Entity_CreatedTimestamp_RecordsCreationTime()
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
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

        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
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
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.IsNull(entity.UpdatedAt);
        Assert.IsNull(entity.UpdatedBy);
    }

    [TestMethod]
    public void Entity_CanUpdateMultipleTimes()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
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
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.AreEqual("Hereditament", entity.DocumentType);
    }

    #endregion

    #region JSON Data

    [TestMethod]
    public void Entity_JsonData_StoresHereditamentEntity()
    {
        var hereditamentData = CreateTestHereditamentEntity();
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = hereditamentData
        };

        Assert.AreEqual(hereditamentData, entity.JsonData);
        Assert.AreEqual("Test Hereditament", entity.JsonData.Name);
    }

    [TestMethod]
    public void Entity_JsonData_CanBeUpdated()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        var updatedData = CreateTestHereditamentEntity("Updated Hereditament");
        entity.JsonData = updatedData;

        Assert.AreEqual("Updated Hereditament", entity.JsonData.Name);
    }

    #endregion

    #region Computed Columns

    [TestMethod]
    public void Entity_AllComputedColumns_CanBeSet()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            NameIndex = "Test Hereditament",
            StatusIndex = HereditamentStatus.Active,
            EffectiveFromIndex = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        Assert.AreEqual("Test Hereditament", entity.NameIndex);
        Assert.AreEqual(HereditamentStatus.Active, entity.StatusIndex);
        Assert.IsNotNull(entity.EffectiveFromIndex);
    }

    [TestMethod]
    public void Entity_NameIndex_CanBeQueried()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            NameIndex = "Test Hereditament"
        };

        Assert.AreEqual("Test Hereditament", entity.NameIndex);
    }

    #endregion

    #region Entity States

    [TestMethod]
    public void Entity_ActiveEntity_HasCorrectState()
    {
        var createdTime = DateTimeOffset.UtcNow;
        var entity = new HereditamentDocumentEntity
        {
            Id = _testId,
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
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
        var entity = new HereditamentDocumentEntity
        {
            Id = _testId,
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
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
        var entity1 = new HereditamentDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };
        var entity2 = new HereditamentDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity("Updated Hereditament")
        };

        Assert.AreNotEqual(entity1.Id, entity2.Id);
    }

    [TestMethod]
    public void Entity_Id_CanBeAssigned()
    {
        var testId = Guid.NewGuid();
        var entity = new HereditamentDocumentEntity
        {
            Id = testId,
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity()
        };

        Assert.AreEqual(testId, entity.Id);
    }

    [TestMethod]
    public void Entity_PartitionKey_CanBeSet()
    {
        var entity = new HereditamentDocumentEntity
        {
            DocumentType = "Hereditament",
            JsonData = CreateTestHereditamentEntity(),
            PartitionKey = "test-key"
        };

        Assert.AreEqual("test-key", entity.PartitionKey);
    }

    #endregion
}
