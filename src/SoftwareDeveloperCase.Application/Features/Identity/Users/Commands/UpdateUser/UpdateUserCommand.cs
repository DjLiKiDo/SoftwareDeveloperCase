using MediatR;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user
/// </summary>
public class UpdateUserCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user password
    /// </summary>
    public string? Password { get; set; }
}
