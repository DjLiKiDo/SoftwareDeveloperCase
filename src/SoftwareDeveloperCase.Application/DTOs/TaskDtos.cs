#nullable enable

namespace SoftwareDeveloperCase.Application.DTOs;

// Request DTOs
public record CreateTaskRequest(
    string Title,
    string Description,
    int ProjectId,
    int? AssignedUserId,
    string Priority,
    int? ParentTaskId,
    DateTime? DueDate);

public record UpdateTaskRequest(
    string Title,
    string Description,
    string Status,
    string Priority,
    int? AssignedUserId,
    DateTime? DueDate);

public record UpdateTaskStatusRequest(string Status);

public record AssignTaskRequest(int UserId);

public record CreateTaskCommentRequest(string Content);

// Response DTOs
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
    List<TaskDto> Subtasks,
    List<TaskCommentDto> RecentComments,
    TimeSpan TotalTimeLogged,
    int CommentCount);

public record TaskDto( // Assuming TaskDto is also needed, based on PagedResult<TaskDto>
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    int ProjectId,
    int? AssignedUserId,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record TaskCommentDto(
    int Id,
    string Content,
    int TaskId,
    int UserId,
    string UserName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record TimeEntryDto(
    int Id,
    int TaskId,
    string TaskTitle,
    int UserId,
    string UserName,
    DateTime StartTime,
    DateTime? EndTime,
    TimeSpan Duration,
    string? Description,
    DateTime CreatedAt);
