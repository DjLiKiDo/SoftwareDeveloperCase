namespace SoftwareDeveloperCase.Application.Contracts.Services;

/// <summary>
/// Service for handling password hashing and verification operations
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes a plain text password using BCrypt with work factor 12
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>The hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain text password against a hashed password
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The hashed password to verify against</param>
    /// <returns>True if the password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Checks if a password needs to be rehashed (e.g., work factor changed)
    /// </summary>
    /// <param name="hashedPassword">The hashed password to check</param>
    /// <returns>True if the password needs rehashing, false otherwise</returns>
    bool NeedsRehash(string hashedPassword);
}