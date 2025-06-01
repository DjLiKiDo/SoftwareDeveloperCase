#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

public record CreateTaskRequest(
    string Title,
    string Description,
    int ProjectId,
    int? AssignedUserId,
    string Priority,
    int? ParentTaskId,
    DateTime? DueDate);
