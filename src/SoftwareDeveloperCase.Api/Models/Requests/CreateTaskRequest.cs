namespace SoftwareDeveloperCase.Api.Models.Requests;

public record CreateTaskRequest(
    string Title,
    string Description,
    int? AssignedUserId,
    string Priority,
    int? ParentTaskId);
