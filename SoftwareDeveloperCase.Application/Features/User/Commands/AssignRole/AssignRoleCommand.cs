using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole
{
    public class AssignRoleCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
