using Address.Infrastructure.Persistence;

namespace Address.Infrastructure.Tests;

[TestClass]
public class AddressDbContextTests
{
    [TestMethod]
    public async Task CanAddAddressToInMemoryDb()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AddressDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddAddress")
            .Options;

        using var context = new AddressDbContext(options);
        var address = new Domain.Entities.AddressEntity
        {
            Uprn = "10001",
            SingleLineAddress = "1 High St",
            BuildingName = "High Street Building",
            BuildingNumber = "1",
            Street = "High St",
            Locality = "Southwark",
            Town = "London",
            Postcode = "SE1 1AA"
        };

        // Act
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Addresses.FirstOrDefaultAsync(a => a.Uprn == "10001");
        saved.Should().NotBeNull();
        saved!.SingleLineAddress.Should().Be("1 High St");
    }
}
