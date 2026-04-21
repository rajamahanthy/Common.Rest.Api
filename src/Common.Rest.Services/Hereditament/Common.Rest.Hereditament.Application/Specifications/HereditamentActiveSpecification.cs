namespace Common.Rest.Hereditament.Application.Specifications;

public class HereditamentActiveSpecification : Specification<HereditamentDocumentEntity>
{
    private readonly string _documentType;
    public HereditamentActiveSpecification(string documentType)
    {
        _documentType = documentType;
    }
    public override Expression<Func<HereditamentDocumentEntity, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType;
    }
}
