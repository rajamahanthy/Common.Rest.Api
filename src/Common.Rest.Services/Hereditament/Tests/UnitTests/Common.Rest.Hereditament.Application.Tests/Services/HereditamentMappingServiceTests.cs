namespace Common.Rest.Hereditament.Application.Tests.Services;

[TestClass]
public class HereditamentMappingServiceTests
{
    private readonly HereditamentMappingService _service = new();

    private static DocumentEntity<HereditamentEntity> CreateDocument(Guid? id = null)
    {
        return new DocumentEntity<HereditamentEntity>
        {
            Id = id ?? Guid.NewGuid(),
            DocumentType = "Hereditament",
            JsonData = new HereditamentEntity
            {
                UARN = Guid.NewGuid(),
                Name = "Test Hereditament",
                Status = HereditamentStatus.Active,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                AddressId = Guid.NewGuid()
            }
        };
    }

    #region MapToDto

    [TestMethod]
    public void MapToDto_NullDocument_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(null!));
    }

    [TestMethod]
    public void MapToDto_NullJsonData_ThrowsArgumentNullException()
    {
        var doc = new DocumentEntity<HereditamentEntity>
        {
            DocumentType = "Hereditament",
            JsonData = null!
        };
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(doc));
    }

    [TestMethod]
    public void MapToDto_ValidDocument_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var doc = new DocumentEntity<HereditamentEntity>
        {
            Id = id,
            DocumentType = "Hereditament",
            JsonData = new HereditamentEntity
            {
                UARN = Guid.NewGuid(),
                Name = "Test Hereditament",
                Status = HereditamentStatus.Active,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
                AddressId = addressId
            }
        };

        var result = _service.MapToDto(doc);

        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("Test Hereditament", result.HereditamentDetails.Name);
        Assert.AreEqual(HereditamentStatus.Active, result.HereditamentDetails.Status);
        Assert.AreEqual(addressId, result.HereditamentDetails.AddressId);
    }

    #endregion

    #region MapToDomain

    [TestMethod]
    public void MapToDomain_NullDto_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(null!));
    }

    [TestMethod]
    public void MapToDomain_ValidDto_ReturnsDocument()
    {
        var addressId = Guid.NewGuid();
        var dto = new CreateUpdateHereditament
        {
            Name = "New Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = addressId
        };

        var result = _service.MapToDomain(dto);

        Assert.AreEqual("Hereditament", result.DocumentType);
        Assert.IsNotNull(result.JsonData);
        Assert.AreEqual("New Hereditament", result.JsonData.Name);
        Assert.AreEqual(HereditamentStatus.Draft, result.JsonData.Status);
        Assert.AreEqual(addressId, result.JsonData.AddressId);
    }

    #endregion

    #region UpdateDomain

    [TestMethod]
    public void UpdateDomain_NullDocument_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.UpdateDomain(null!, new CreateUpdateHereditament
            {
                Name = "Updated",
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow)
            }));
    }

    [TestMethod]
    public void UpdateDomain_NullUpdateDto_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.UpdateDomain(CreateDocument(), null!));
    }

    [TestMethod]
    public void UpdateDomain_Valid_ReturnsUpdatedDocument()
    {
        var existing = CreateDocument();
        var existingUarn = existing.JsonData!.UARN;
        var addressId = Guid.NewGuid();
        var updateDto = new CreateUpdateHereditament
        {
            Name = "Updated Name",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            AddressId = addressId
        };

        var result = _service.UpdateDomain(existing, updateDto);

        Assert.AreEqual(existingUarn, result.JsonData!.UARN);
        Assert.AreEqual("Updated Name", result.JsonData.Name);
        Assert.AreEqual(HereditamentStatus.Active, result.JsonData.Status);
        Assert.AreEqual(updateDto.EffectiveFrom, result.JsonData.EffectiveFrom);
        Assert.AreEqual(addressId, result.JsonData.AddressId);
    }

    #endregion
}
