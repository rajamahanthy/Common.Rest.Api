namespace Address.Application.Tests;

[TestClass]
public class AddressServiceTests
{
    private readonly IRepository<AddressEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAddressMappingService _mappingService;
    private readonly AddressService _sut;

    public AddressServiceTests()
    {
        _repository = Substitute.For<IRepository<AddressEntity>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mappingService = new AddressMappingService();

        _sut = new AddressService(_repository, _unitOfWork, _mappingService);
    }

    [TestMethod]
    public async Task GetAddressByIdAsync_WhenAddressExists_ShouldReturnAddressDto()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var address = new AddressEntity { Id = addressId, SingleLineAddress = "123 Test St" };
        _repository.GetByIdAsync(addressId, Arg.Any<CancellationToken>()).Returns(address);

        // Act
        var result = await _sut.GetAddressByIdAsync(addressId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(addressId);
    }
}
