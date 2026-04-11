namespace Common.Rest.Address.Application.Specifications;

public class AddressAdvancedSearchSpecification : Specification<AddressDocumentEntity>
{
    private readonly string? _postcode;
    private readonly string? _postTown;
    private readonly string? _organisation;
    private readonly string? _thoroughfare;
    private readonly string? _locality;
    private readonly string _documentType;

    public AddressAdvancedSearchSpecification(
        string documentType,
        string? postcode = null,
        string? postTown = null,
        string? organisation = null,
        string? thoroughfare = null,
        string? locality = null)
    {
        _documentType = documentType;
        _postcode = postcode;
        _postTown = postTown;
        _organisation = organisation;
        _thoroughfare = thoroughfare;
        _locality = locality;
    }

    public override Expression<Func<AddressDocumentEntity, bool>> ToExpression()
    {
        return d =>
            !d.IsDeleted &&
            d.DocumentType == _documentType &&

            (string.IsNullOrWhiteSpace(_postcode) ||
                (d.PostcodeIndex != null &&
                    d.PostcodeIndex.ToLower() == _postcode.Trim().ToLower())) &&

            (string.IsNullOrWhiteSpace(_postTown) ||
                (d.PostTownIndex != null &&
                    d.PostTownIndex.ToLower().Contains(_postTown.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_organisation) ||
                (d.OrganisationIndex != null &&
                    d.OrganisationIndex.ToLower().Contains(_organisation.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_thoroughfare) ||
                (d.ThoroughfareIndex != null &&
                    d.ThoroughfareIndex.ToLower().Contains(_thoroughfare.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_locality) ||
                (
                    (d.LocalityIndex != null &&
                        d.LocalityIndex.ToLower().Contains(_locality.Trim().ToLower()))
                    ||
                    (d.DependentLocalityIndex != null &&
                        d.DependentLocalityIndex.ToLower().Contains(_locality.Trim().ToLower()))
                ));
    }
}
