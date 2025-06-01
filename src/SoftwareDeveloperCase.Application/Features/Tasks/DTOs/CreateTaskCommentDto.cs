namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for creating a task comment
/// </summary>
public class CreateTaskCommentDto
{
    /// <summary>
    /// Task ID the comment belongs to
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
