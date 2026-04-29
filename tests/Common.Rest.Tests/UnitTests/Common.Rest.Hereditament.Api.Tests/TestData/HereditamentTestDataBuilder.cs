namespace Common.Rest.Hereditament.Api.Tests.TestData;

/// <summary>
/// Generic test data builder for creating Hereditament DTOs and related objects.
/// Minimizes duplication across test classes.
/// </summary>
public static class HereditamentTestDataBuilder
{
    public static HereditamentDocumentDto CreateHereditamentDocumentDto(Guid? id = null)
    {
        return new HereditamentDocumentDto(
            id ?? Guid.NewGuid(),
            CreateHereditamentDto()
        );
    }

    public static HereditamentDto CreateHereditamentDto()
    {
        return new HereditamentDto
        {
            Name = "Test Hereditament",
            Status = HereditamentStatus.Active,
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
    }

    public static CreateUpdateHereditament CreateUpdateHereditamentDto()
    {
        return new CreateUpdateHereditament
        {
            Name = "Test Hereditament",
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
    }

    public static HereditamentEntity CreateHereditamentEntity(string name = "Test Hereditament")
    {
        return new HereditamentEntity
        {
            UARN = Guid.NewGuid(),
            Name = name,
            Status = HereditamentStatus.Active,
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            AddressId = Guid.NewGuid()
        };
    }
}
