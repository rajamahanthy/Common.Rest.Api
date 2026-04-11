using System;
using System.Linq.Expressions;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Specification;

namespace Common.Rest.Address.Application.Specifications;

public class AddressUprnSpecification : Specification<AddressDocumentEntity>
{
    private readonly string _uprn;
    private readonly string _documentType;
    public AddressUprnSpecification(string documentType, string uprn)
    {
        _uprn = uprn;
        _documentType = documentType;
    }
    public override Expression<Func<AddressDocumentEntity, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType && d.UprnIndex == _uprn;
    }
}
