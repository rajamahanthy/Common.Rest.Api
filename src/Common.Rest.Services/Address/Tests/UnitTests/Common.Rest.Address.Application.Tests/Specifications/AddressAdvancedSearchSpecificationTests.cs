namespace Common.Rest.Address.Application.Tests.Specifications;

[TestClass]
public class AddressAdvancedSearchSpecificationTests
{
    private static DocumentEntity<AddressEntity> CreateEntity(
        string? postcode = null,
        string? postTown = null,
        string? organisation = null,
        string? streetDescription = null,
        string? locality = null,
        string? dependentLocality = null,
        bool isDeleted = false,
        string documentType = "Address")
    {
        return new DocumentEntity<AddressEntity>
        {
            DocumentType = documentType,
            IsDeleted = isDeleted,
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                AddressInfo = new AddressInfoEntity
                {
                    Organisation = organisation ?? "",
                    Pao = new AddressableObjectEntity { Text = "1" },
                    StreetDescriptor = new StreetDescriptorEntity
                    {
                        StreetDescription = streetDescription ?? "",
                        PostTown = postTown ?? "",
                        Locality = locality ?? "",
                        DependentLocality = dependentLocality ?? ""
                    },
                    Postcode = postcode ?? ""
                },
                Geography = new GeographyEntity { Easting = 1, Northing = 2 }
            }
        };
    }

    [TestMethod]
    public void ToExpression_NoFilters_MatchesAllActive()
    {
        var spec = new AddressAdvancedSearchSpecification("Address");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "T1 1ST");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_PostcodeMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postcode: "T1 1ST");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "T1 1ST");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_PostcodeCaseInsensitive_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postcode: "t1 1st");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "T1 1ST");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_PostcodeMismatch_ReturnsFalse()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postcode: "T1 1ST");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "B2 2ND");
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_PostTownMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postTown: "London");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postTown: "London");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_PostTownContains_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postTown: "Lon");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postTown: "London");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_OrganisationMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", organisation: "Acme Corp");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(organisation: "Acme Corp");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_ThoroughfareMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", thoroughfare: "Main Street");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(streetDescription: "Main Street");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_LocalityMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", locality: "Testville");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(locality: "Testville");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_DependentLocalityMatch_ReturnsTrue()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", locality: "DepLoc");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(dependentLocality: "DepLoc");
        Assert.IsTrue(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_DeletedEntity_ReturnsFalse()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postcode: "T1 1ST");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "T1 1ST", isDeleted: true);
        Assert.IsFalse(compiled(entity));
    }

    [TestMethod]
    public void ToExpression_WrongDocumentType_ReturnsFalse()
    {
        var spec = new AddressAdvancedSearchSpecification("Address", postcode: "T1 1ST");
        var compiled = spec.ToExpression().Compile();

        var entity = CreateEntity(postcode: "T1 1ST", documentType: "Hereditament");
        Assert.IsFalse(compiled(entity));
    }
}
