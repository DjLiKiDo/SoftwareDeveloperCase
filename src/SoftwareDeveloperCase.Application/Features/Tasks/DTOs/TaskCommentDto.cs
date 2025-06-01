#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for task comments
/// </summary>
/// <param name="Id">Unique identifier of the comment</param>
/// <param name="Content">The content of the comment</param>
/// <param name="TaskId">The ID of the task this comment belongs to</param>
/// <param name="UserId">The ID of the user who created the comment</param>
/// <param name="UserName">The name of the user who created the comment</param>
/// <param name="CreatedAt">The date and time when the comment was created</param>
/// <param name="UpdatedAt">Optional date and time when the comment was last updated</param>
public record TaskCommentDto(
    int Id,
    string Content,
    int TaskId,
    int UserId,
    string UserName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
