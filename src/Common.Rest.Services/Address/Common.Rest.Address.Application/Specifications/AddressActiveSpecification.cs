using System;
using System.Linq.Expressions;
using Common.Rest.Address.Domain.Entities;
using Common.Rest.Shared.Specification;

namespace Common.Rest.Address.Application.Specifications;

public class AddressActiveSpecification : Specification<AddressDocumentEntity>
{
    private readonly string _documentType;
    public AddressActiveSpecification(string documentType)
    {
        _documentType = documentType;
    }
    public override Expression<Func<AddressDocumentEntity, bool>> ToExpression()
    {
        return d => !d.IsDeleted && d.DocumentType == _documentType;
    }
}
