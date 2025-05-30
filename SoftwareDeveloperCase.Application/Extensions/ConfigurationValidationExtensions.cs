using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Application.Validation;

namespace SoftwareDeveloperCase.Application.Extensions;

/// <summary>
/// Extension methods for configuration validation
/// </summary>
public static class ConfigurationValidationExtensions
{
    /// <summary>
    /// Validates EmailSettings configuration and returns validation results
    /// </summary>
    /// <param name="emailSettings">The email settings to validate</param>
    /// <returns>ValidateOptionsResult containing validation results</returns>
    public static ValidateOptionsResult ValidateEmailSettings(this EmailSettings emailSettings)
    {
        var validator = new EmailSettingsValidator();
        return validator.Validate(null, emailSettings);
    }

    /// <summary>
    /// Validates DatabaseSettings configuration and returns validation results
    /// </summary>
    /// <param name="databaseSettings">The database settings to validate</param>
    /// <returns>ValidateOptionsResult containing validation results</returns>
    public static ValidateOptionsResult ValidateDatabaseSettings(this DatabaseSettings databaseSettings)
    {
        var validator = new DatabaseSettingsValidator();
        return validator.Validate(null, databaseSettings);
    }
}