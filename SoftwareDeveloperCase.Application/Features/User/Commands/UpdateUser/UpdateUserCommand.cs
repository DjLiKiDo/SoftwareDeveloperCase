using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
