#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Request DTO for creating a comment on a task
/// </summary>
/// <param name="Content">The content of the comment</param>
public record CreateTaskCommentRequest(string Content);
