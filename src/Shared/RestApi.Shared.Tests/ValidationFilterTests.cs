using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestApi.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RestApi.Shared.Tests;

[TestClass]
public class ValidationFilterTests
{
    [TestMethod]
    public void ValidationFilter_PlaceholderTest()
    {
        // Simple test to ensure test project runs
        true.Should().BeTrue();
    }
}
