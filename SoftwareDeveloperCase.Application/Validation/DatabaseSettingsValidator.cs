using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace SoftwareDeveloperCase.Application.Validation;

/// <summary>
/// Validator for DatabaseSettings configuration using IValidateOptions pattern
/// </summary>
public class DatabaseSettingsValidator : IValidateOptions<DatabaseSettings>
{
    /// <summary>
    /// Validates the DatabaseSettings configuration
    /// </summary>
    /// <param name="name">The name of the options instance being validated</param>
    /// <param name="options">The options instance to validate</param>
    /// <returns>ValidateOptionsResult indicating success or failure</returns>
    public ValidateOptionsResult Validate(string? name, DatabaseSettings options)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(options);

        if (!Validator.TryValidateObject(options, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error").ToList();
            return ValidateOptionsResult.Fail(errors);
        }

        // Additional custom validation
        if (!options.UseInMemoryDatabase && string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail("ConnectionString is required when UseInMemoryDatabase is false.");
        }

        var validProviders = new[] { "InMemory", "SqlServer", "SQLite", "PostgreSQL" };
        if (!validProviders.Contains(options.DatabaseProvider, StringComparer.OrdinalIgnoreCase))
        {
            return ValidateOptionsResult.Fail($"DatabaseProvider must be one of: {string.Join(", ", validProviders)}");
        }

        if (options.CommandTimeoutSeconds <= 0)
        {
            return ValidateOptionsResult.Fail("CommandTimeoutSeconds must be greater than 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
