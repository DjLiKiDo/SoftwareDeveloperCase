#nullable enable

namespace SoftwareDeveloperCase.Application.Features.Tasks.DTOs;

// Simple TaskDto for basic task information
public record SimpleTaskDto(
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
