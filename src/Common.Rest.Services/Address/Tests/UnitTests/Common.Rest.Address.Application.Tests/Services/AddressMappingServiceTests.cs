namespace Common.Rest.Address.Application.Tests.Services;

[TestClass]
public class AddressMappingServiceTests
{
    private readonly AddressMappingService _service = new();

    private static DocumentEntity<AddressEntity> CreateDocument(Guid? id = null)
    {
        return new DocumentEntity<AddressEntity>
        {
            Id = id ?? Guid.NewGuid(),
            DocumentType = "Address",
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                Usrn = "999999999",
                AddressInfo = new AddressInfoEntity
                {
                    Organisation = "Test Org",
                    Department = "Test Dept",
                    Sao = new AddressableObjectEntity
                    {
                        Text = "SAO Text",
                        StartNumber = 1,
                        StartSuffix = "A",
                        EndNumber = 5,
                        EndSuffix = "B"
                    },
                    Pao = new AddressableObjectEntity
                    {
                        Text = "PAO Text",
                        StartNumber = 10,
                        StartSuffix = "C",
                        EndNumber = 20,
                        EndSuffix = "D"
                    },
                    StreetDescriptor = new StreetDescriptorEntity
                    {
                        StreetDescription = "Main St",
                        Locality = "Locality",
                        DependentLocality = "Dep Locality",
                        DoubleDependentLocality = "Double Dep",
                        TownName = "Town",
                        PostTown = "PostTown",
                        AdministrativeArea = "Admin Area"
                    },
                    Postcode = "T1 1ST"
                },
                Geography = new GeographyEntity { Easting = 529904, Northing = 180994 }
            }
        };
    }

    #region MapToDto

    [TestMethod]
    public void MapToDto_NullDocument_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(null!));
    }

    [TestMethod]
    public void MapToDto_NullJsonData_ThrowsArgumentNullException()
    {
        var doc = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = null!
        };
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(doc));
    }

    [TestMethod]
    public void MapToDto_NullAddressInfo_ThrowsArgumentNullException()
    {
        var doc = new DocumentEntity<AddressEntity>
        {
            DocumentType = "Address",
            JsonData = new AddressEntity
            {
                Uprn = "123456789",
                AddressInfo = null!,
                Geography = new GeographyEntity { Easting = 1, Northing = 2 }
            }
        };
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(doc));
    }

    [TestMethod]
    public void MapToDto_ValidDocument_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var doc = CreateDocument(id);

        var result = _service.MapToDto(doc);

        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("123456789", result.AddressDetails.Uprn);
        Assert.AreEqual("Test Org", result.AddressDetails.AddressInfo.Organisation);
        Assert.AreEqual("Main St", result.AddressDetails.AddressInfo.StreetDescriptor.StreetDescription);
        Assert.AreEqual("T1 1ST", result.AddressDetails.AddressInfo.Postcode);
        Assert.AreEqual(529904, result.AddressDetails.Geography.Easting);
        Assert.AreEqual(180994, result.AddressDetails.Geography.Northing);
    }

    [TestMethod]
    public void MapToDto_NullSao_MapsSaoAsNull()
    {
        var doc = CreateDocument();
        doc.JsonData!.AddressInfo!.Sao = null;

        var result = _service.MapToDto(doc);

        Assert.IsNull(result.AddressDetails.AddressInfo.Sao);
    }

    #endregion

    #region MapToDomain

    [TestMethod]
    public void MapToDomain_NullDto_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(null!));
    }

    [TestMethod]
    public void MapToDomain_NullAddressInfo_ThrowsArgumentNullException()
    {
        var dto = new CreateUpdateAddress
        {
            AddressInfo = null!,
            Geography = new Geography { Easting = 1, Northing = 2 }
        };
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(dto));
    }

    [TestMethod]
    public void MapToDomain_NullPao_ThrowsArgumentNullException()
    {
        var dto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = null!,
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Main St" },
                Postcode = "T1 1ST"
            },
            Geography = new Geography { Easting = 1, Northing = 2 }
        };
        Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(dto));
    }

    [TestMethod]
    public void MapToDomain_ValidDto_ReturnsDocument()
    {
        var dto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Organisation = "Org",
                Department = "Dept",
                Sao = new AddressableObject
                {
                    Text = "SAO Text",
                    StartNumber = 1,
                    StartSuffix = "A",
                    EndNumber = 5,
                    EndSuffix = "B"
                },
                Pao = new AddressableObject
                {
                    Text = "PAO Text",
                    StartNumber = 10,
                    StartSuffix = "C",
                    EndNumber = 20,
                    EndSuffix = "D"
                },
                StreetDescriptor = new StreetDescriptor
                {
                    StreetDescription = "Main St",
                    Locality = "Locality",
                    DependentLocality = "Dep Loc",
                    DoubleDependentLocality = "Double Dep",
                    TownName = "Town",
                    PostTown = "PostTown",
                    AdministrativeArea = "Admin Area"
                },
                Postcode = "T1 1ST"
            },
            Geography = new Geography { Easting = 529904, Northing = 180994 }
        };

        var result = _service.MapToDomain(dto);

        Assert.AreEqual("Address", result.DocumentType);
        Assert.IsNotNull(result.JsonData);
        Assert.IsNotNull(result.JsonData.Uprn);
        Assert.AreEqual("Org", result.JsonData.AddressInfo!.Organisation);
        Assert.AreEqual("Main St", result.JsonData.AddressInfo.StreetDescriptor.StreetDescription);
    }

    [TestMethod]
    public void MapToDomain_NullSao_MapsSaoAsNull()
    {
        var dto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "PAO" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "St" },
                Postcode = "T1 1ST"
            },
            Geography = new Geography { Easting = 1, Northing = 2 }
        };

        var result = _service.MapToDomain(dto);

        Assert.IsNull(result.JsonData!.AddressInfo!.Sao);
    }

    #endregion

    #region UpdateDomain

    [TestMethod]
    public void UpdateDomain_NullDocument_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.UpdateDomain(null!, new CreateUpdateAddress
            {
                AddressInfo = new AddressInfo
                {
                    Pao = new AddressableObject(),
                    StreetDescriptor = new StreetDescriptor(),
                    Postcode = "T1 1ST"
                },
                Geography = new Geography()
            }));
    }

    [TestMethod]
    public void UpdateDomain_NullUpdateDto_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.UpdateDomain(CreateDocument(), null!));
    }

    [TestMethod]
    public void UpdateDomain_Valid_ReturnsUpdatedDocument()
    {
        var existing = CreateDocument();
        var updateDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Organisation = "Updated Org",
                Pao = new AddressableObject { Text = "Updated PAO" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "Updated St" },
                Postcode = "B2 2ND"
            },
            Geography = new Geography { Easting = 100, Northing = 200 }
        };

        var result = _service.UpdateDomain(existing, updateDto);

        Assert.AreEqual(existing.JsonData!.Uprn, result.JsonData!.Uprn);
        Assert.AreEqual("Updated Org", result.JsonData.AddressInfo!.Organisation);
        Assert.AreEqual("Updated St", result.JsonData.AddressInfo.StreetDescriptor.StreetDescription);
        Assert.AreEqual("B2 2ND", result.JsonData.AddressInfo.Postcode);
        Assert.AreEqual(100, result.JsonData.Geography!.Easting);
        Assert.AreEqual(200, result.JsonData.Geography.Northing);
    }

    [TestMethod]
    public void UpdateDomain_NullGeography_PreservesExistingGeography()
    {
        var existing = CreateDocument();
        var updateDto = new CreateUpdateAddress
        {
            AddressInfo = new AddressInfo
            {
                Pao = new AddressableObject { Text = "PAO" },
                StreetDescriptor = new StreetDescriptor { StreetDescription = "St" },
                Postcode = "T1 1ST"
            },
            Geography = null!
        };

        var result = _service.UpdateDomain(existing, updateDto);

        Assert.AreEqual(529904, result.JsonData!.Geography!.Easting);
        Assert.AreEqual(180994, result.JsonData.Geography.Northing);
    }

    #endregion
}
