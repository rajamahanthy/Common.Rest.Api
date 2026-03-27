namespace Address.Application.Tests;

[TestClass]
public class AddressServiceTests
{
    private readonly IRepository<Address.Domain.Entities.Address> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AddressService _sut;

    public AddressServiceTests()
    {
        _repository = Substitute.For<IRepository<Address.Domain.Entities.Address>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AddressMappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new AddressService(_repository, _unitOfWork, _mapper);
    }

    [TestMethod]
    public async Task GetAddressByIdAsync_WhenAddressExists_ShouldReturnAddressDto()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var address = new Address.Domain.Entities.Address { Id = addressId, SingleLineAddress = "123 Test St" };
        _repository.GetByIdAsync(addressId, Arg.Any<CancellationToken>()).Returns(address);

        // Act
        var result = await _sut.GetAddressByIdAsync(addressId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(addressId);
    }
}
