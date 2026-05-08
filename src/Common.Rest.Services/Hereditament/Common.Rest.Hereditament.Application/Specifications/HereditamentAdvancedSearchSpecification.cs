namespace Common.Rest.Hereditament.Application.Specifications;

public class HereditamentAdvancedSearchSpecification : Specification< DocumentEntity<HereditamentEntity>>
{
    private readonly string? _name;
    private readonly string? _status;
    private readonly DateOnly? _effectiveFrom;
    private readonly string _documentType;

    public HereditamentAdvancedSearchSpecification(
        string documentType,
        string? name = null,
        string? status = null,
        DateOnly? effectiveFrom = null)
    {
        _documentType = documentType;
        _name = name;
        _status = status;
        _effectiveFrom = effectiveFrom;
    }

    public override Expression<Func< DocumentEntity<HereditamentEntity>, bool>> ToExpression()
    {
        return d =>
            !d.IsDeleted &&
            d.DocumentType == _documentType &&

            (string.IsNullOrWhiteSpace(_name) ||
                (d.JsonData != null &&
                    d.JsonData.Name.ToLower() == _name.Trim().ToLower())) &&

            (string.IsNullOrWhiteSpace(_status) ||
                (d.JsonData != null &&
                    d.JsonData.Status.ToLower().Contains(_status.Trim().ToLower()))) &&

            (_effectiveFrom == null ||
                (d.JsonData != null &&
                    d.JsonData.EffectiveFrom.Equals(_effectiveFrom))); 
    }
}

