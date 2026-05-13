namespace Common.Rest.Address.Application.Specifications;

public class AddressUprnSpecification : Specification<DocumentEntity<AddressEntity>>
{
    private readonly string _uprn;
    private readonly string _documentType;
    public AddressUprnSpecification(string documentType, string uprn)
    {
        _uprn = uprn;
        _documentType = documentType;
    }
    public override Expression<Func<DocumentEntity<AddressEntity>, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType && 
               d.JsonData != null && d.JsonData.Uprn == _uprn;
    }
}
