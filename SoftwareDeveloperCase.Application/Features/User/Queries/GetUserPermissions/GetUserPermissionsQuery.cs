using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions
{
    public class GetUserPermissionsQuery : IRequest<List<PermissionDto>>
    {
        public Guid UserId { get; set; }

        public GetUserPermissionsQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
