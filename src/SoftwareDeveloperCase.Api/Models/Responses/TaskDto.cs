namespace SoftwareDeveloperCase.Api.Models.Responses;

public record TaskDto(
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
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
