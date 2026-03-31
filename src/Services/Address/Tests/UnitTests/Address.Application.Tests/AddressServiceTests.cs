namespace Address.Application.Tests;

[TestClass]
public class AddressServiceTests
{
    private readonly Mock<IRepository<AddressEntity>> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IAddressMappingService _mappingService;
    private readonly AddressService _sut;

    public AddressServiceTests()
    {
        _repositoryMock = new Mock<IRepository<AddressEntity>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mappingService = new AddressMappingService();

        _sut = new AddressService(_repositoryMock.Object, _unitOfWorkMock.Object, _mappingService);
    }

    [TestMethod]
    public async Task GetAddressByIdAsync_WhenAddressExists_ShouldReturnAddressDto()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var address = new AddressEntity { Id = addressId, SingleLineAddress = "123 Test St" };
        _repositoryMock.Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        // Act
        var result = await _sut.GetAddressByIdAsync(addressId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(addressId);
    }
}
