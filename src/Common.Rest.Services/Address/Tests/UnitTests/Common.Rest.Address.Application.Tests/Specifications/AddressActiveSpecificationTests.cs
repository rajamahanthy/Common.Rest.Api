namespace Common.Rest.Address.Application.Tests.Specifications;

[TestClass]
public class AddressActiveSpecificationTests
{
    [TestMethod]
    public void ToExpression_ActiveAddressWithCorrectDocumentType_ReturnsTrue()
    {
        var spec = new AddressActiveSpecification("Address");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            IsDeleted = false,
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                AddressInfo = new AddressInfoEntity
                {
                    Pao = new AddressableObjectEntity { Text = "1" },
                    StreetDescriptor = new StreetDescriptorEntity { StreetDescription = "Main St" },
                    Postcode = "T1 1ST"
                },
                Geography = new GeographyEntity { Easting = 1, Northing = 2 }
            }
        };

        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_WrongDocumentType_ReturnsFalse()
    {
        var spec = new AddressActiveSpecification("Address");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Hereditament",
            IsDeleted = false,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_Deleted_ReturnsFalse()
    {
        var spec = new AddressActiveSpecification("Address");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            IsDeleted = true,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_DeletedAndWrongType_ReturnsFalse()
    {
        var spec = new AddressActiveSpecification("Address");
        var compiled = spec.ToExpression().Compile();

        var entity = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Other",
            IsDeleted = true,
            JsonData = null!
        };

        Assert.IsFalse(compiled(entity));
    }
}
