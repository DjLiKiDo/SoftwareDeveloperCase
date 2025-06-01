namespace SoftwareDeveloperCase.Application.Validation.Common;

/// <summary>
/// Common error messages for consistent validation feedback
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Error message for required fields
    /// </summary>
    public const string Required = "{PropertyName} is required.";
    
    /// <summary>
    /// Error message for invalid email format
    /// </summary>
    public const string InvalidEmail = "{PropertyName} must be a valid email address.";
    
    /// <summary>
    /// Error message for fields with length constraints
    /// </summary>
    public const string InvalidLength = "{PropertyName} must be between {MinLength} and {MaxLength} characters.";
    
    /// <summary>
    /// Error message for fields that must be unique
    /// </summary>
    public const string MustBeUnique = "{PropertyName} already exists.";
    
    /// <summary>
    /// Error message for fields with specific format requirements
    /// </summary>
    public const string InvalidFormat = "{PropertyName} has an invalid format.";
    
    /// <summary>
    /// Error message for numeric fields that must be positive
    /// </summary>
    public const string MustBePositive = "{PropertyName} must be a positive value.";
    
    /// <summary>
    /// Error message for invalid date ranges
    /// </summary>
    public const string InvalidDateRange = "End date must be after start date.";
    
    /// <summary>
    /// Error message for references to non-existent entities
    /// </summary>
    public const string NotFound = "{PropertyName} does not exist.";
}
