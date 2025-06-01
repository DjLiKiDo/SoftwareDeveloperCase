namespace SoftwareDeveloperCase.Application.Attributes;

/// <summary>
/// Attribute to mark properties that should be excluded from automatic sanitization
/// Used for sensitive data like passwords, tokens, etc. that should not be modified
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SkipSanitizationAttribute : Attribute
{
    /// <summary>
    /// Gets the reason why sanitization should be skipped
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkipSanitizationAttribute"/> class
    /// </summary>
    /// <param name="reason">The reason why sanitization should be skipped</param>
    public SkipSanitizationAttribute(string reason = "Sensitive data that should not be modified")
    {
        Reason = reason;
    }
}
