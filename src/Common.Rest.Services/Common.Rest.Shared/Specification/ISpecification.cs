namespace Common.Rest.Shared.Specification;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}
