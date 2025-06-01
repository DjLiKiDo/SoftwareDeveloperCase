namespace SoftwareDeveloperCase.Application.DTOs.Auth;

/// <summary>
/// User information for authentication response
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
