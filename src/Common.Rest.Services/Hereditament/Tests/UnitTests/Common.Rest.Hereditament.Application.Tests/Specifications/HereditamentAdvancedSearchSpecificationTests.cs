namespace Common.Rest.Hereditament.Application.Tests.Specifications;

[TestClass]
public class HereditamentAdvancedSearchSpecificationTests
{
    private static DocumentEntity<HereditamentEntity> CreateEntity(
        string? name = null,
        string? status = null,
        DateOnly? effectiveFrom = null,
        bool isDeleted = false,
        string documentType = "Hereditament")
    {
        return new DocumentEntity<HereditamentEntity>
        {
            DocumentType = documentType,
            IsDeleted = isDeleted,
            JsonData = new HereditamentEntity
            {
                UARN = Guid.NewGuid(),
                Name = name ?? "Test",
                Status = status ?? HereditamentStatus.Active,
                EffectiveFrom = effectiveFrom ?? DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };
    }

    [TestMethod]
    public void ToExpression_NoFilters_MatchesAllActive()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity();
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_NameMatch_ReturnsTrue()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", name: "TestName");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(name: "TestName");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_NameCaseSensitive_ReturnsTrue()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", name: "testname");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(name: "testname");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_NameMismatch_ReturnsFalse()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", name: "Expected");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(name: "Actual");
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_StatusMatch_ReturnsTrue()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", status: "Active");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(status: "Active");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_StatusContains_ReturnsTrue()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", status: "cti");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(status: "Active");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_StatusMismatch_ReturnsFalse()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", status: "Removed");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(status: "Draft");
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_EffectiveFromMatch_ReturnsTrue()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", effectiveFrom: date);
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(effectiveFrom: date);
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_EffectiveFromMismatch_ReturnsFalse()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", effectiveFrom: DateOnly.FromDateTime(DateTime.UtcNow));
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(effectiveFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)));
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_DeletedEntity_ReturnsFalse()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(isDeleted: true);
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_WrongDocumentType_ReturnsFalse()
    {
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(documentType: "Address");
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_AllFiltersMatch_ReturnsTrue()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var spec = new HereditamentAdvancedSearchSpecification("Hereditament", "TestName", "Active", date);
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(name: "TestName", status: "Active", effectiveFrom: date);
        Assert.IsTrue(compiled(entity));
    }
}
