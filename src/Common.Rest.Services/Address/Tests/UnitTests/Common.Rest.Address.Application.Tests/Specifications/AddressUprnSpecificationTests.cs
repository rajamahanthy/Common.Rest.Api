namespace Common.Rest.Address.Application.Tests.Specifications;

[TestClass]
public class AddressUprnSpecificationTests
{
    private static DocumentEntity<AddressEntity> CreateEntity(string uprn, string documentType = "Address", bool isDeleted = false)
    {
        return new DocumentEntity<AddressEntity>
        {
            DocumentType = documentType,
            IsDeleted = isDeleted,
            JsonData = new AddressEntity
            {
                Uprn = uprn,
                AddressInfo = new AddressInfoEntity
                {
                    Pao = new AddressableObjectEntity { Text = "1" },
                    StreetDescriptor = new StreetDescriptorEntity { StreetDescription = "Main St" },
                    Postcode = "T1 1ST"
                },
                Geography = new GeographyEntity { Easting = 1, Northing = 2 }
            }
        };
    }

    [TestMethod]
    public void ToExpression_MatchingUprn_ReturnsTrue()
    {
        var spec = new AddressUprnSpecification("Address", "123456789");
        var compiled = spec.ToExpression().Compile();

        Assert.IsTrue(compiled(CreateEntity("123456789")));
    }

    [TestMethod]
    public void ToExpression_NonMatchingUprn_ReturnsFalse()
    {
        var spec = new AddressUprnSpecification("Address", "123456789");
        var compiled = spec.ToExpression().Compile();

        Assert.IsFalse(compiled(CreateEntity("999999999")));
    }

    [TestMethod]
    public void ToExpression_WrongDocumentType_ReturnsFalse()
    {
        var spec = new AddressUprnSpecification("Address", "123456789");
        var compiled = spec.ToExpression().Compile();

        Assert.IsFalse(compiled(CreateEntity("123456789", documentType: "Hereditament")));
    }

    [TestMethod]
    public void ToExpression_Deleted_ReturnsFalse()
    {
        var spec = new AddressUprnSpecification("Address", "123456789");
        var compiled = spec.ToExpression().Compile();

        Assert.IsFalse(compiled(CreateEntity("123456789", isDeleted: true)));
    }

    [TestMethod]
    public void ToExpression_NullJsonData_ReturnsFalse()
    {
        var spec = new AddressUprnSpecification("Address", "123456789");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            IsDeleted = false,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }
}
