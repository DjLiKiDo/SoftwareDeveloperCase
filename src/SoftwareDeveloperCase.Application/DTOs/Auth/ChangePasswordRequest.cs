using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.DTOs.Auth;

/// <summary>
/// Request for changing user password
/// </summary>
public class ChangePasswordRequest
{
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
