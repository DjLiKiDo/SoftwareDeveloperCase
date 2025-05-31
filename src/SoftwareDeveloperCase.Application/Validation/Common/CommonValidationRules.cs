using FluentValidation;
using SoftwareDeveloperCase.Domain.ValueObjects;

namespace SoftwareDeveloperCase.Application.Validation.Common;

/// <summary>
/// Common validation rules that can be reused across validators
/// </summary>
public static class CommonValidationRules
{
    /// <summary>
    /// Validates email format and constraints
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(254).WithMessage("Email cannot exceed 254 characters.")
            .Must(BeValidEmailFormat).WithMessage("Email format is invalid.");
    }

    /// <summary>
    /// Validates password strength
    /// </summary>
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
    }

    /// <summary>
    /// Validates name fields (first name, last name, etc.)
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Name can only contain letters, spaces, hyphens, apostrophes, and periods.");
    }

    /// <summary>
    /// Validates description fields
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidDescription<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }

    /// <summary>
    /// Validates that a date is not in the future
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> NotInFuture<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");
    }

    /// <summary>
    /// Validates that a date is not in the past
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> NotInPast<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Date cannot be in the past.");
    }

    /// <summary>
    /// Validates ID fields (must be positive)
    /// </summary>
    public static IRuleBuilderOptions<T, int> ValidId<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("ID must be a positive value.");
    }

    /// <summary>
    /// Validates that end date is after start date
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> AfterStartDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder, DateTime startDate)
    {
        return ruleBuilder
            .GreaterThan(startDate).WithMessage("End date must be after start date.");
    }

    /// <summary>
    /// Private method to validate email format using domain value object
    /// </summary>
    private static bool BeValidEmailFormat(string email)
    {
        try
        {
            var _ = new Email(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
