#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

public record UpdateTaskRequest(
    string Title,
    string Description,
    string Status,
    string Priority,
    int? AssignedUserId,
    DateTime? DueDate);
