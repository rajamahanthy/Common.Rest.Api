using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Address.Domain.Tests;

[TestClass]
public class AddressEntityTests
{
    [TestMethod]
    public void Address_ShouldSetDefaultCountry()
    {
        // Act
        var address = new Entities.Address();

        // Assert
        address.Country.Should().Be("United Kingdom");
    }

    [TestMethod]
    public void Address_ShouldInheritFromBaseEntity()
    {
        // Act
        var address = new Entities.Address();

        // Assert
        address.Should().BeAssignableTo<RestApi.Shared.Domain.BaseEntity>();
        address.Id.Should().NotBeEmpty();
    }
}
