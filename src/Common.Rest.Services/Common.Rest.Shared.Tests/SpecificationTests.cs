namespace Common.Rest.Shared.Tests;

public class TestSpecification : Specification<string>
{
    private readonly string _expected;
    public TestSpecification(string expected) => _expected = expected;
    public override Expression<Func<string, bool>> ToExpression()
        => s => s == _expected;
}

[TestClass]
public class SpecificationTests
{
    [TestMethod]
    public void ToExpression_ReturnsCorrectExpression()
    {
        var spec = new TestSpecification("hello");
        var expr = spec.ToExpression();
        var compiled = expr.Compile();

        Assert.IsTrue(compiled("hello"));
        Assert.IsFalse(compiled("world"));
    }

    [TestMethod]
    public void AndOperator_CreatesAndSpecification()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("hello");
        var and = left & right;

        Assert.IsInstanceOfType(and, typeof(AndSpecification<string>));
    }

    [TestMethod]
    public void OrOperator_CreatesOrSpecification()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("world");
        var or = left | right;

        Assert.IsInstanceOfType(or, typeof(OrSpecification<string>));
    }
}

[TestClass]
public class AndSpecificationTests
{
    [TestMethod]
    public void AndSpecification_BothTrue_ReturnsTrue()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("hello");
        var and = new AndSpecification<string>(left, right);

        var compiled = and.ToExpression().Compile();
        Assert.IsTrue(compiled("hello"));
    }

    [TestMethod]
    public void AndSpecification_OneFalse_ReturnsFalse()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("world");
        var and = new AndSpecification<string>(left, right);

        var compiled = and.ToExpression().Compile();
        Assert.IsFalse(compiled("hello"));
        Assert.IsFalse(compiled("world"));
        Assert.IsFalse(compiled("other"));
    }

    [TestMethod]
    public void AndSpecification_BothFalse_ReturnsFalse()
    {
        var left = new TestSpecification("foo");
        var right = new TestSpecification("bar");
        var and = new AndSpecification<string>(left, right);

        var compiled = and.ToExpression().Compile();
        Assert.IsFalse(compiled("baz"));
    }
}

[TestClass]
public class OrSpecificationTests
{
    [TestMethod]
    public void OrSpecification_EitherTrue_ReturnsTrue()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("world");
        var or = new OrSpecification<string>(left, right);

        var compiled = or.ToExpression().Compile();
        Assert.IsTrue(compiled("hello"));
        Assert.IsTrue(compiled("world"));
    }

    [TestMethod]
    public void OrSpecification_BothFalse_ReturnsFalse()
    {
        var left = new TestSpecification("foo");
        var right = new TestSpecification("bar");
        var or = new OrSpecification<string>(left, right);

        var compiled = or.ToExpression().Compile();
        Assert.IsFalse(compiled("baz"));
    }

    [TestMethod]
    public void OrSpecification_BothTrue_ReturnsTrue()
    {
        var left = new TestSpecification("hello");
        var right = new TestSpecification("hello");
        var or = new OrSpecification<string>(left, right);

        var compiled = or.ToExpression().Compile();
        Assert.IsTrue(compiled("hello"));
    }
}
