namespace SurveyData.Domain.Tests;

[TestClass]
public class SurveyEntityTests
{
    [TestMethod]
    public void Survey_ShouldAllowAddingDetails()
    {
        // Arrange
        var survey = new Survey { ReferenceNumber = "REF-001" };
        var detail = new SurveyDetail { PropertyType = "House", Description = "A nice house" };

        // Act
        survey.Details.Add(detail);

        // Assert
        Assert.Contains(detail, survey.Details);
        Assert.Equals(detail.Survey, survey);
    }
}
