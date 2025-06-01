#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Request DTO for updating an existing task
/// </summary>
/// <param name="Title">The updated title of the task</param>
/// <param name="Description">The updated description of the task</param>
/// <param name="Status">The updated status of the task</param>
/// <param name="Priority">The updated priority of the task</param>
/// <param name="AssignedUserId">The updated assigned user ID</param>
/// <param name="DueDate">The updated due date of the task</param>
public record UpdateTaskRequest(
    string Title,
    string Description,
    string Status,
    string Priority,
    int? AssignedUserId,
    DateTime? DueDate);
