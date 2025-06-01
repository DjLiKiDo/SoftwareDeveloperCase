using MediatR;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command for user login
/// </summary>
public class LoginCommand : IRequest<AuthenticationResponse>
{
    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password
    /// </summary>
    [SkipSanitization("Password should not be sanitized as it could alter the authentication credential")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommand"/> class
    /// </summary>
    public LoginCommand() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommand"/> class
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="password">The user's password</param>
    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}