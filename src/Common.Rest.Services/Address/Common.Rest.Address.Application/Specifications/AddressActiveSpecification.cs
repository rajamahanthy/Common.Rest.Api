namespace Common.Rest.Address.Application.Specifications;

public class AddressActiveSpecification : Specification<DocumentEntity<AddressEntity>>
{
    private readonly string _documentType;
    public AddressActiveSpecification(string documentType)
    {
        _documentType = documentType;
    }
    public override Expression<Func<DocumentEntity<AddressEntity>, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType;
    }
}
