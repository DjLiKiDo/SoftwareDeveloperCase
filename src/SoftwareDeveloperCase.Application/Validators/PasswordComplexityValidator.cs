using FluentValidation;

namespace SoftwareDeveloperCase.Application.Validators;

/// <summary>
/// Validator for password complexity requirements
/// </summary>
public static class PasswordComplexityValidator
{
    /// <summary>
    /// Defines password complexity rules for FluentValidation
    /// </summary>
    /// <param name="ruleBuilder">The rule builder for the password property</param>
    /// <returns>The rule builder with password complexity rules applied</returns>
    public static IRuleBuilderOptions<T, string?> PasswordComplexity<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character")
            .Must(NotContainCommonPasswords).WithMessage("Password is too common, please choose a stronger password");
    }

    /// <summary>
    /// Checks if the password is not in the list of common passwords
    /// </summary>
    /// <param name="password">The password to check</param>
    /// <returns>True if the password is not common, false otherwise</returns>
    private static bool NotContainCommonPasswords(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // List of common passwords to reject
        var commonPasswords = new[]
        {
            "password", "123456", "123456789", "qwerty", "abc123", "monkey",
            "letmein", "dragon", "111111", "baseball", "iloveyou", "trustno1",
            "1234567", "sunshine", "master", "123123", "welcome", "shadow",
            "ashley", "football", "jesus", "michael", "ninja", "mustang",
            "password1", "password123", "admin", "root", "user", "test",
            "guest", "123", "1234", "12345", "pass", "passw0rd", "p@ssw0rd"
        };

        // Check for exact matches (case insensitive) or if password contains common password
        return !commonPasswords.Any(common => 
            password.Contains(common, StringComparison.OrdinalIgnoreCase));
    }
}