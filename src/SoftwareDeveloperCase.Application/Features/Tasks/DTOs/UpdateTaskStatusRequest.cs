#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Request DTO for updating only the status of a task
/// </summary>
/// <param name="Status">The new status of the task</param>
public record UpdateTaskStatusRequest(string Status);
