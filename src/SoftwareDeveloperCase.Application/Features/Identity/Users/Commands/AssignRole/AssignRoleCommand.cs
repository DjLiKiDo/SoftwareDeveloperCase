using MediatR;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;

/// <summary>
/// Command to assign a role to a user
/// </summary>
public class AssignRoleCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public Guid RoleId { get; set; }
}
