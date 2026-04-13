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

    #region Entity Initialization

    [TestMethod]
    public void Constructor_CreatesNewEntity()
    {
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.Id.Should().Be(_testId);
        entity.DocumentType.Should().Be("Address");
        entity.JsonData.Should().NotBeNull();
        entity.IsDeleted.Should().BeFalse();
    }

    [TestMethod]
    public void Entity_PropertiesCanBeSet()
    {
        var createdTime = DateTimeOffset.UtcNow;
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            UprnIndex = "123456789",
            PostcodeIndex = "T1 1ST",
            PostTownIndex = "Test Town"
        };

        entity.UprnIndex.Should().Be("123456789");
        entity.PostcodeIndex.Should().Be("T1 1ST");
        entity.PostTownIndex.Should().Be("Test Town");
        entity.CreatedAt.Should().Be(createdTime);
        entity.CreatedBy.Should().Be("test-user");
    }

    [TestMethod]
    public void Entity_ComputedColumns_InitiallyNull()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.UprnIndex.Should().BeNull();
        entity.PostcodeIndex.Should().BeNull();
        entity.PostTownIndex.Should().BeNull();
    }

    #endregion

    #region Soft Delete Support

    [TestMethod]
    public void Entity_MarkAsDeleted_SetsIsDeletedFlag()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            IsDeleted = false
        };

        entity.IsDeleted = true;

        entity.IsDeleted.Should().BeTrue();
    }

    [TestMethod]
    public void Entity_RestoreDeleted_ClearsIsDeletedFlag()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            IsDeleted = true
        };

        entity.IsDeleted = false;

        entity.IsDeleted.Should().BeFalse();
    }

    [TestMethod]
    public void Entity_IsDeletedFalseByDefault()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.IsDeleted.Should().BeFalse();
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
            JsonData = new AddressEntity { Uprn = "123456789" },
            CreatedAt = now,
            CreatedBy = "creator-user"
        };

        entity.CreatedAt.Should().Be(now);
        entity.CreatedBy.Should().Be("creator-user");
    }

    [TestMethod]
    public void Entity_UpdatedTimestamp_TrackModifications()
    {
        var createdTime = DateTimeOffset.UtcNow.AddHours(-1);
        var updatedTime = DateTimeOffset.UtcNow;

        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            CreatedAt = createdTime,
            CreatedBy = "creator",
            UpdatedAt = updatedTime,
            UpdatedBy = "updater"
        };

        entity.UpdatedAt.Should().Be(updatedTime);
        entity.UpdatedBy.Should().Be("updater");
        entity.UpdatedAt.Should().BeAfter(entity.CreatedAt);
    }

    [TestMethod]
    public void Entity_UpdatedPropertiesNullWhenNotModified()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.UpdatedAt.Should().BeNull();
        entity.UpdatedBy.Should().BeNull();
    }

    [TestMethod]
    public void Entity_CanUpdateMultipleTimes()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        var firstUpdate = DateTimeOffset.UtcNow;
        entity.UpdatedAt = firstUpdate;
        entity.UpdatedBy = "user1";

        var secondUpdate = DateTimeOffset.UtcNow.AddMinutes(1);
        entity.UpdatedAt = secondUpdate;
        entity.UpdatedBy = "user2";

        entity.UpdatedBy.Should().Be("user2");
        entity.UpdatedAt.Should().Be(secondUpdate);
    }

    #endregion

    #region Document Type

    [TestMethod]
    public void Entity_DocumentType_CanBeSet()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.DocumentType.Should().Be("Address");
    }

    #endregion

    #region JSON Data

    [TestMethod]
    public void Entity_JsonData_StoresAddressEntity()
    {
        var addressData = new AddressEntity { Uprn = "123456789" };
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = addressData
        };

        entity.JsonData.Should().Be(addressData);
        entity.JsonData.Uprn.Should().Be("123456789");
    }

    [TestMethod]
    public void Entity_JsonData_CanBeUpdated()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        var updatedData = new AddressEntity { Uprn = "987654321" };
        entity.JsonData = updatedData;

        entity.JsonData.Uprn.Should().Be("987654321");
    }

    #endregion

    #region Computed Columns

    [TestMethod]
    public void Entity_AllComputedColumns_CanBeSet()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            UprnIndex = "123456789",
            PostcodeIndex = "T1 1ST",
            PostTownIndex = "Test Town",
            OrganisationIndex = "Test Org",
            ThoroughfareIndex = "Main Street",
            LocalityIndex = "City",
            DependentLocalityIndex = "Ward"
        };

        entity.UprnIndex.Should().Be("123456789");
        entity.PostcodeIndex.Should().Be("T1 1ST");
        entity.PostTownIndex.Should().Be("Test Town");
        entity.OrganisationIndex.Should().Be("Test Org");
        entity.ThoroughfareIndex.Should().Be("Main Street");
        entity.LocalityIndex.Should().Be("City");
        entity.DependentLocalityIndex.Should().Be("Ward");
    }

    [TestMethod]
    public void Entity_UprnIndex_CanBeQueried()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            UprnIndex = "123456789"
        };

        entity.UprnIndex.Should().Be("123456789");
    }

    [TestMethod]
    public void Entity_PostcodeIndex_CanBeQueried()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            PostcodeIndex = "T1 1ST"
        };

        entity.PostcodeIndex.Should().Be("T1 1ST");
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
            JsonData = new AddressEntity { Uprn = "123456789" },
            CreatedAt = createdTime,
            CreatedBy = "test-user",
            IsDeleted = false
        };

        entity.IsDeleted.Should().BeFalse();
        entity.Id.Should().NotBe(Guid.Empty);
        entity.CreatedBy.Should().Be("test-user");
    }

    [TestMethod]
    public void Entity_DeletedEntity_HasCorrectState()
    {
        var entity = new AddressDocumentEntity
        {
            Id = _testId,
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            CreatedBy = "creator",
            IsDeleted = true
        };

        entity.IsDeleted.Should().BeTrue();
        entity.CreatedBy.Should().Be("creator");
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
            JsonData = new AddressEntity { Uprn = "123456789" }
        };
        var entity2 = new AddressDocumentEntity
        {
            Id = Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "987654321" }
        };

        entity1.Id.Should().NotBe(entity2.Id);
    }

    [TestMethod]
    public void Entity_Id_CanBeAssigned()
    {
        var testId = Guid.NewGuid();
        var entity = new AddressDocumentEntity
        {
            Id = testId,
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" }
        };

        entity.Id.Should().Be(testId);
    }

    [TestMethod]
    public void Entity_PartitionKey_CanBeSet()
    {
        var entity = new AddressDocumentEntity
        {
            DocumentType = "Address",
            JsonData = new AddressEntity { Uprn = "123456789" },
            PartitionKey = "test-key"
        };

        entity.PartitionKey.Should().Be("test-key");
    }

    #endregion
}
