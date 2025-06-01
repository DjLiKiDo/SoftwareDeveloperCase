#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

/// <summary>
/// Request DTO for assigning a task to a user
/// </summary>
/// <param name="UserId">The ID of the user to assign the task to</param>
public record AssignTaskRequest(int UserId);
