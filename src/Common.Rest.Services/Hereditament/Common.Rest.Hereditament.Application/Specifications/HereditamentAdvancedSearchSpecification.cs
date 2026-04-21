namespace Common.Rest.Hereditament.Application.Specifications;

public class HereditamentAdvancedSearchSpecification : Specification<HereditamentDocumentEntity>
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

    public override Expression<Func<HereditamentDocumentEntity, bool>> ToExpression()
    {
        return d =>
            !d.IsDeleted &&
            d.DocumentType == _documentType &&

            (string.IsNullOrWhiteSpace(_name) ||
                (d.NameIndex != null &&
                    d.NameIndex.ToLower() == _name.Trim().ToLower())) &&

            (string.IsNullOrWhiteSpace(_status) ||
                (d.StatusIndex != null &&
                    d.StatusIndex.ToLower().Contains(_status.Trim().ToLower()))) &&

            (_effectiveFrom == null ||
                (d.EffectiveFromIndex != null &&
                    d.EffectiveFromIndex.Equals(_effectiveFrom))); 
    }
}
