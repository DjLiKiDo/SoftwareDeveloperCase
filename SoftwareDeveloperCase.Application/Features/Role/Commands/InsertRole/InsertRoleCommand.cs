using MediatR;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole
{
    public class InsertRoleCommand : IRequest<Guid>
    {
        public string? Name { get; set; }

        public Guid? ParentRoleId { get; set; }
    }
}
