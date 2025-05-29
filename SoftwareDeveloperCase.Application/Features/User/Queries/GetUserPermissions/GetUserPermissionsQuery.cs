using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions
{
    /// <summary>
    /// Query to get all permissions for a specific user
    /// </summary>
    public class GetUserPermissionsQuery : IRequest<List<PermissionDto>>
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
}
