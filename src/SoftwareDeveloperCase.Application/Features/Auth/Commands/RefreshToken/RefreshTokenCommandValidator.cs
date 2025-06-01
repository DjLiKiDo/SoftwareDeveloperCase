using FluentValidation;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Validator for refresh token command with security-focused validation
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandValidator"/> class
    /// </summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required")
            .Must(BeValidToken)
            .WithMessage("Invalid refresh token format");
    }

    /// <summary>
    /// Validates if the refresh token has a valid format
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    private static bool BeValidToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // Sanitize the token to ensure it doesn't contain malicious content
        var sanitized = InputSanitizer.SanitizeString(token);
        
        // Token should remain unchanged after sanitization
        // and should be at least 32 characters (typical for secure tokens)
        return sanitized == token && token.Length >= 32;
    }
}