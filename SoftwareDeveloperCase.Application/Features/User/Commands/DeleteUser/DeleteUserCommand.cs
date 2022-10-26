using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
