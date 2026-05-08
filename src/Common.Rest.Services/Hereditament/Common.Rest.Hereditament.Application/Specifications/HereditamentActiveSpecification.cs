namespace Common.Rest.Hereditament.Application.Specifications;

public class HereditamentActiveSpecification : Specification< DocumentEntity<HereditamentEntity>>
{
    private readonly string _documentType;
    public HereditamentActiveSpecification(string documentType)
    {
        _documentType = documentType;
    }
    public override Expression<Func< DocumentEntity<HereditamentEntity>, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType;
    }
}
