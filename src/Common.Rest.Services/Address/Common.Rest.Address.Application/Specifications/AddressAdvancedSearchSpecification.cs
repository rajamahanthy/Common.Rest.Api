namespace Common.Rest.Address.Application.Specifications;

public class AddressAdvancedSearchSpecification : Specification<DocumentEntity<AddressEntity>>
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

    public override Expression<Func<DocumentEntity<AddressEntity>, bool>> ToExpression()
    {
        return d =>
            !d.IsDeleted &&
            d.DocumentType == _documentType &&

            (string.IsNullOrWhiteSpace(_postcode) ||
                (d.JsonData != null && d.JsonData.AddressInfo != null &&
                    d.JsonData.AddressInfo.Postcode != null &&
                    d.JsonData.AddressInfo.Postcode.ToLower() == _postcode.Trim().ToLower())) &&

            (string.IsNullOrWhiteSpace(_postTown) ||
                (d.JsonData != null && d.JsonData.AddressInfo != null && d.JsonData.AddressInfo.StreetDescriptor != null &&
                    d.JsonData.AddressInfo.StreetDescriptor.PostTown != null &&
                    d.JsonData.AddressInfo.StreetDescriptor.PostTown.ToLower().Contains(_postTown.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_organisation) ||
                (d.JsonData != null && d.JsonData.AddressInfo != null &&
                    d.JsonData.AddressInfo.Organisation != null &&
                    d.JsonData.AddressInfo.Organisation.ToLower().Contains(_organisation.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_thoroughfare) ||
                (d.JsonData != null && d.JsonData.AddressInfo != null && d.JsonData.AddressInfo.StreetDescriptor != null &&
                    d.JsonData.AddressInfo.StreetDescriptor.StreetDescription != null &&
                    d.JsonData.AddressInfo.StreetDescriptor.StreetDescription.ToLower().Contains(_thoroughfare.Trim().ToLower()))) &&

            (string.IsNullOrWhiteSpace(_locality) ||
                (
                    (d.JsonData != null && d.JsonData.AddressInfo != null && d.JsonData.AddressInfo.StreetDescriptor != null &&
                        d.JsonData.AddressInfo.StreetDescriptor.Locality != null &&
                        d.JsonData.AddressInfo.StreetDescriptor.Locality.ToLower().Contains(_locality.Trim().ToLower()))
                    ||
                    (d.JsonData != null && d.JsonData.AddressInfo != null && d.JsonData.AddressInfo.StreetDescriptor != null &&
                        d.JsonData.AddressInfo.StreetDescriptor.DependentLocality != null &&
                        d.JsonData.AddressInfo.StreetDescriptor.DependentLocality.ToLower().Contains(_locality.Trim().ToLower()))
                ));
    }
}
