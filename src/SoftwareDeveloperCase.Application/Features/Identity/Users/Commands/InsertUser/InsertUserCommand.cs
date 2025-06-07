using MediatR;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;

/// <summary>
/// Command to register a new user in the system
/// </summary>
public class InsertUserCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// User's full name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// User's email address (must be unique)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    public string? Password { get; set; }
}
