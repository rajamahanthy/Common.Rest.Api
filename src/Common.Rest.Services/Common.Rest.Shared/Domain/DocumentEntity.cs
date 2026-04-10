namespace Common.Rest.Shared.Domain;

/// <summary>
/// Represents a generic document with flexible JSON storage and system audit fields.
/// </summary>
/// <typeparam name="TData">The type of data to be stored as JSON.</typeparam>
public class DocumentEntity<TData> : BaseEntity
    where TData : class
{
    /// <summary>
    /// Gets or sets the document type classifier.
    /// </summary>
    public required string DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the typed data object.
    /// Automatically serialized to and deserialized from JSON in the database.
    /// </summary>
    public required TData JsonData { get; set; }

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency control.
    /// </summary>
    public byte[]? RowVersion { get; set; }
}

