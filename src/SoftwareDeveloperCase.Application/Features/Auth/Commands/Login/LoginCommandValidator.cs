using FluentValidation;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validator for login command with security-focused validation
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidator"/> class
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .Must(BeValidEmail)
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters");
    }

    /// <summary>
    /// Validates if the email is in a valid format and safe after sanitization
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>True if the email is valid, false otherwise</returns>
    private static bool BeValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var sanitized = InputSanitizer.SanitizeEmail(email);
        return sanitized != null && sanitized.Equals(email, StringComparison.OrdinalIgnoreCase);
    }
}
