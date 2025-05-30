using MediatR;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user
/// </summary>
public class UpdateUserCommand : IRequest<Guid>
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

    /// <summary>
    /// Gets or sets the department identifier
    /// </summary>
    public Guid DepartmentId { get; set; }
}
