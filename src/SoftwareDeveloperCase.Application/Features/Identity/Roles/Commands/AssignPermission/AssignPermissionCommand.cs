using MediatR;

namespace SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;

/// <summary>
/// Command to assign a permission to a role
/// </summary>
public class AssignPermissionCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the permission identifier
    /// </summary>
    public Guid PermissionId { get; set; }
}
