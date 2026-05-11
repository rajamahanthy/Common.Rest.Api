using System.ComponentModel.DataAnnotations;

namespace Common.Rest.Shared.Tests;

[TestClass]
public class AnyOfAttributeTests
{
    [TestMethod]
    public void Constructor_WithNullProperties_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AnyOfAttribute(null!));
    }

    [TestMethod]
    public void Constructor_WithEmptyProperties_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AnyOfAttribute());
    }

    [TestMethod]
    public void IsValid_NullValue_ReturnsSuccess()
    {
        var attr = new AnyOfAttribute("Name");
        var result = attr.GetValidationResult(null, new ValidationContext(new object()));
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_AtLeastOnePropertyPresent_ReturnsSuccess()
    {
        var model = new AnyOfTestModel { Name = "John" };
        var context = new ValidationContext(model);
        var attr = new AnyOfAttribute(nameof(AnyOfTestModel.Name), nameof(AnyOfTestModel.Email));

        var result = attr.GetValidationResult(model, context);
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_AllPropertiesEmpty_ReturnsError()
    {
        var model = new AnyOfTestModel { Name = "", Email = "" };
        var context = new ValidationContext(model);
        var attr = new AnyOfAttribute(nameof(AnyOfTestModel.Name), nameof(AnyOfTestModel.Email));

        var result = attr.GetValidationResult(model, context);
        Assert.AreNotEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_AllPropertiesNull_ReturnsError()
    {
        var model = new AnyOfTestModel();
        var context = new ValidationContext(model);
        var attr = new AnyOfAttribute(nameof(AnyOfTestModel.Name), nameof(AnyOfTestModel.Email));

        var result = attr.GetValidationResult(model, context);
        Assert.AreNotEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_NonStringPropertyWithValue_ReturnsSuccess()
    {
        var model = new AnyOfInvalidModel { Id = 1, Code = 0 };
        var context = new ValidationContext(model);
        var attr = new AnyOfAttribute(nameof(AnyOfInvalidModel.Id), nameof(AnyOfInvalidModel.Code));

        var result = attr.GetValidationResult(model, context);
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_MissingConfiguredProperty_ReturnsError()
    {
        var model = new AnyOfMissingPropertyModel { Name = "test" };
        var context = new ValidationContext(model);
        var attr = new AnyOfAttribute("Name", "NonExistent");

        var result = attr.GetValidationResult(model, context);
        Assert.AreNotEqual(ValidationResult.Success, result);
    }
}

public class AnyOfTestModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class AnyOfInvalidModel
{
    public int Id { get; set; }
    public int Code { get; set; }
}

public class AnyOfMissingPropertyModel
{
    public string? Name { get; set; }
}

public class NoAdditionalPropertiesValidModel
{
    public string? Name { get; set; }
}

public class NoAdditionalPropertiesInvalidModel
{
    public string? Name { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }
}

[TestClass]
public class NoAdditionalPropertiesAttributeTests
{
    [TestMethod]
    public void IsValid_NullValue_ReturnsSuccess()
    {
        var attr = new NoAdditionalPropertiesAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new object()));
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_NoExtensionData_ReturnsSuccess()
    {
        var model = new NoAdditionalPropertiesValidModel { Name = "test" };
        var context = new ValidationContext(model);
        var attr = new NoAdditionalPropertiesAttribute();

        var result = attr.GetValidationResult(model, context);
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_EmptyExtensionData_ReturnsSuccess()
    {
        var model = new NoAdditionalPropertiesInvalidModel { Name = "test", ExtensionData = new() };
        var context = new ValidationContext(model);
        var attr = new NoAdditionalPropertiesAttribute();

        var result = attr.GetValidationResult(model, context);
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_ExtensionDataPresent_ReturnsError()
    {
        var model = new NoAdditionalPropertiesInvalidModel
        {
            Name = "test",
            ExtensionData = new() { ["extraField"] = "unexpected" }
        };
        var context = new ValidationContext(model);
        var attr = new NoAdditionalPropertiesAttribute();

        var result = attr.GetValidationResult(model, context);
        Assert.AreNotEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_MultipleExtraFields_ReportsAll()
    {
        var model = new NoAdditionalPropertiesInvalidModel
        {
            Name = "test",
            ExtensionData = new()
            {
                ["field1"] = "value1",
                ["field2"] = "value2"
            }
        };
        var context = new ValidationContext(model);
        var attr = new NoAdditionalPropertiesAttribute();

        var result = attr.GetValidationResult(model, context);
        Assert.AreNotEqual(ValidationResult.Success, result);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.ErrorMessage!.Contains("field1"));
        Assert.IsTrue(result.ErrorMessage.Contains("field2"));
    }
}
