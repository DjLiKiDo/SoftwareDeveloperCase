using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities.Core;

/// <summary>
/// Represents a comment on a task.
/// </summary>
public class TaskComment : BaseEntity
{
    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the task identifier this comment belongs to.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the task this comment belongs to.
    /// </summary>
    public Task Task { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user identifier who created this comment.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the user who created this comment.
    /// </summary>
    public User Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
