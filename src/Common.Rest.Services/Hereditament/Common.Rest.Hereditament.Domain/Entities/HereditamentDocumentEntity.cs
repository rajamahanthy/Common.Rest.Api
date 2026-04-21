namespace Common.Rest.Hereditament.Domain.Entities;

using Common.Rest.Shared.Domain;

/// <summary>
/// Represents an Hereditament document with computed columns for efficient JSON property querying.
/// </summary>
public class HereditamentDocumentEntity : DocumentEntity<HereditamentEntity>
{
    /// <summary>
    /// Computed column: Extracted Name from JsonData for efficient querying.
    /// Maps to: $.Hereditament.name
    /// </summary>
    public string? NameIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted Status from JsonData for efficient querying.
    /// Maps to: $.Hereditament.status
    /// </summary>
    public string? StatusIndex { get; set; }

    /// <summary>
    /// Computed column: Extracted EffectiveFrom from JsonData for efficient querying.
    /// Maps to: $.Hereditament.effectiveFrom
    /// </summary>
    public DateOnly? EffectiveFromIndex { get; set; }
    
}
