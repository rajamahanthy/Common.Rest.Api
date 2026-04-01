using System.Collections.ObjectModel;

namespace Common.Rest.SurveyData.Domain.Entities;

/// <summary>
/// Custom collection for SurveyDetail that automatically manages parent-child relationships.
/// </summary>
public class SurveyDetailCollection : Collection<SurveyDetail>
{
    private Survey _survey;

    public SurveyDetailCollection(Survey survey)
    {
        _survey = survey;
    }

    protected override void InsertItem(int index, SurveyDetail item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // Set the parent reference and foreign key
        item.Survey = _survey;
        item.SurveyId = _survey.Id;

        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, SurveyDetail item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // Set the parent reference and foreign key
        item.Survey = _survey;
        item.SurveyId = _survey.Id;

        base.SetItem(index, item);
    }
}
