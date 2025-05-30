namespace SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions;

/// <summary>
/// Data transfer object for permission information
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Gets or sets the permission identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the permission name
    /// </summary>
    public string? Name { get; set; }
}
