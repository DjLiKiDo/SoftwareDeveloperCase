using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.DeleteUser;

/// <summary>
/// Command to delete a user
/// </summary>
public class DeleteUserCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the user identifier to delete
    /// </summary>
    public Guid Id { get; set; }
}
