using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser;

/// <summary>
/// Command to register a new user in the system
/// </summary>
public class InsertUserCommand : IRequest<Guid>
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

    /// <summary>
    /// Department ID where the user belongs
    /// </summary>
    public Guid DepartmentId { get; set; }
}
