using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.Entities.Identity;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the token value
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID that owns this refresh token
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user that owns this refresh token
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the expiration date of the refresh token
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the date when the token was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the JTI (JWT ID) of the access token this refresh token was used with
    /// </summary>
    public string? JwtId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the refresh token is active (not expired and not revoked)
    /// </summary>
    public bool IsActive => !IsRevoked && DateTime.UtcNow <= ExpiresAt;
}