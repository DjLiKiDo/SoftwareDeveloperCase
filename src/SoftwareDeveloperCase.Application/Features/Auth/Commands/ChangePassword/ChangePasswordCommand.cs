using MediatR;
using SoftwareDeveloperCase.Application.Attributes;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.ChangePassword;

/// <summary>
/// Command for changing user password
/// </summary>
public class ChangePasswordCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Gets or sets the user ID (populated by controller)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the current password
    /// </summary>
    [SkipSanitization("Password should not be sanitized as it could alter the authentication credential")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password
    /// </summary>
    [SkipSanitization("Password should not be sanitized as it could alter the authentication credential")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation of the new password
    /// </summary>
    [SkipSanitization("Password should not be sanitized as it could alter the authentication credential")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
