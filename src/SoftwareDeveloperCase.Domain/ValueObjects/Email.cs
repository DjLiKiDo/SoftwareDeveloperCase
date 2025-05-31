using SoftwareDeveloperCase.Domain.Common;
using System.Text.RegularExpressions;

namespace SoftwareDeveloperCase.Domain.ValueObjects;

/// <summary>
/// Represents an email address value object.
/// </summary>
public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the email address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Email"/> class.
    /// </summary>
    /// <param name="value">The email address value.</param>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid.</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format.", nameof(value));

        Value = value.ToLowerInvariant();
    }

    /// <summary>
    /// Gets the domain part of the email address.
    /// </summary>
    public string Domain => Value.Split('@')[1];

    /// <summary>
    /// Gets the local part of the email address.
    /// </summary>
    public string LocalPart => Value.Split('@')[0];

    /// <summary>
    /// Implicitly converts a string to an Email.
    /// </summary>
    /// <param name="email">The email string.</param>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Explicitly converts a string to an Email.
    /// </summary>
    /// <param name="email">The email string.</param>
    public static explicit operator Email(string email) => new(email);

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The email value for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the email.
    /// </summary>
    /// <returns>The email value as a string.</returns>
    public override string ToString() => Value;
}
