using FluentValidation;

namespace SoftwareDeveloperCase.Application.Validation.Common;

/// <summary>
/// Base validator class with common validation rules and error messages
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Validates that a string is not empty and within length constraints
    /// </summary>
    protected void ValidateRequiredString(IRuleBuilder<T, string> ruleBuilder, int minLength = 1, int maxLength = 255)
    {
        ruleBuilder
            .NotEmpty().WithMessage(ErrorMessages.Required)
            .Length(minLength, maxLength).WithMessage(ErrorMessages.InvalidLength);
    }

    /// <summary>
    /// Validates email format using built-in EmailAddress validator
    /// </summary>
    protected void ValidateEmail(IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty().WithMessage(ErrorMessages.Required)
            .EmailAddress().WithMessage(ErrorMessages.InvalidEmail)
            .MaximumLength(254).WithMessage(ErrorMessages.InvalidLength);
    }

    /// <summary>
    /// Validates that a value is positive
    /// </summary>
    protected void ValidatePositiveValue<TProperty>(IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : struct, IComparable<TProperty>
    {
        ruleBuilder
            .Must(value => value.CompareTo(default(TProperty)) > 0)
            .WithMessage(ErrorMessages.MustBePositive);
    }

    /// <summary>
    /// Validates that an ID exists (is greater than 0 for integers)
    /// </summary>
    protected void ValidateId(IRuleBuilder<T, int> ruleBuilder)
    {
        ruleBuilder
            .GreaterThan(0).WithMessage(ErrorMessages.NotFound);
    }
}
