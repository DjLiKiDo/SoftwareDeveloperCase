using MediatR;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.InsertRole;

/// <summary>
/// Command to create a new role
/// </summary>
public class InsertRoleCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the parent role identifier (optional for role hierarchy)
    /// </summary>
    public Guid? ParentRoleId { get; set; }
}
