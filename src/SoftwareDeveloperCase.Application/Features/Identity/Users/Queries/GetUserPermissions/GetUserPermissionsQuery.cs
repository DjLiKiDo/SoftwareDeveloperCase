using MediatR;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;

/// <summary>
/// Query to get all permissions for a specific user
/// </summary>
public class GetUserPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Initializes a new instance of the GetUserPermissionsQuery class
    /// </summary>
    /// <param name="userId">The user identifier</param>
    public GetUserPermissionsQuery(Guid userId)
    {
        UserId = userId;
    }
}
