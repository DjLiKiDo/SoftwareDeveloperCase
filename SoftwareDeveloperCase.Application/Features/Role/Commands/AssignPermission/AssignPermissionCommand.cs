using MediatR;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission
{
    public class AssignPermissionCommand : IRequest<Guid>
    {
        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }
    }
}
