namespace Common.Rest.Hereditament.Application.Tests.Specifications;

[TestClass]
public class HereditamentActiveSpecificationTests
{
    [TestMethod]
    public void ToExpression_ActiveWithCorrectDocumentType_ReturnsTrue()
    {
        var spec = new HereditamentActiveSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<HereditamentEntity>
        {
            DocumentType = "Hereditament",
            IsDeleted = false,
            JsonData = new HereditamentEntity
            {
                UARN = Guid.NewGuid(),
                Name = "Test",
                Status = HereditamentStatus.Active,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_WrongDocumentType_ReturnsFalse()
    {
        var spec = new HereditamentActiveSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<HereditamentEntity>
        {
            DocumentType = "Address",
            IsDeleted = false,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_Deleted_ReturnsFalse()
    {
        var spec = new HereditamentActiveSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<HereditamentEntity>
        {
            DocumentType = "Hereditament",
            IsDeleted = true,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_DeletedAndWrongType_ReturnsFalse()
    {
        var spec = new HereditamentActiveSpecification("Hereditament");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<HereditamentEntity>
        {
            DocumentType = "Other",
            IsDeleted = true,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }
}
