#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

public record TaskCommentDto(
    int Id,
    string Content,
    int TaskId,
    int UserId,
    string UserName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
