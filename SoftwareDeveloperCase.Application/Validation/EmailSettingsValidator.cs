using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace SoftwareDeveloperCase.Application.Validation;

/// <summary>
/// Validator for EmailSettings configuration using IValidateOptions pattern
/// </summary>
public class EmailSettingsValidator : IValidateOptions<EmailSettings>
{
    /// <summary>
    /// Validates the EmailSettings configuration
    /// </summary>
    /// <param name="name">The name of the options instance being validated</param>
    /// <param name="options">The options instance to validate</param>
    /// <returns>ValidateOptionsResult indicating success or failure</returns>
    public ValidateOptionsResult Validate(string? name, EmailSettings options)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(options);

        if (!Validator.TryValidateObject(options, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error").ToList();
            return ValidateOptionsResult.Fail(errors);
        }

        // Additional custom validation
        if (options.SmtpPort <= 0 || options.SmtpPort > 65535)
        {
            return ValidateOptionsResult.Fail("SmtpPort must be between 1 and 65535.");
        }

        if (string.IsNullOrWhiteSpace(options.SmtpServer))
        {
            return ValidateOptionsResult.Fail("SmtpServer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.FromAddress))
        {
            return ValidateOptionsResult.Fail("FromAddress is required.");
        }

        if (string.IsNullOrWhiteSpace(options.FromName))
        {
            return ValidateOptionsResult.Fail("FromName is required.");
        }

        return ValidateOptionsResult.Success;
    }
}