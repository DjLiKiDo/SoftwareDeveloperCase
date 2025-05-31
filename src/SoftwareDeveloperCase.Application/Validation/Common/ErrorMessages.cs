namespace SoftwareDeveloperCase.Application.Validation.Common;

/// <summary>
/// Common error messages for consistent validation feedback
/// </summary>
public static class ErrorMessages
{
    public const string Required = "{PropertyName} is required.";
    public const string InvalidEmail = "{PropertyName} must be a valid email address.";
    public const string InvalidLength = "{PropertyName} must be between {MinLength} and {MaxLength} characters.";
    public const string MustBeUnique = "{PropertyName} already exists.";
    public const string InvalidFormat = "{PropertyName} has an invalid format.";
    public const string MustBePositive = "{PropertyName} must be a positive value.";
    public const string InvalidDateRange = "End date must be after start date.";
    public const string NotFound = "{PropertyName} does not exist.";
}
