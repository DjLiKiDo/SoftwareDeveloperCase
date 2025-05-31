#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

public record TaskDetailDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    int ProjectId,
    string ProjectName,
    int? AssignedUserId,
    string? AssignedUserName,
    int? ParentTaskId,
    string? ParentTaskTitle,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SimpleTaskDto> Subtasks,
    List<TaskCommentDto> RecentComments,
    TimeSpan TotalTimeLogged,
    int CommentCount);
