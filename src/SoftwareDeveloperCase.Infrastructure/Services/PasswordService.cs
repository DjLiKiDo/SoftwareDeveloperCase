using BC = BCrypt.Net.BCrypt;
using SoftwareDeveloperCase.Application.Contracts.Services;

namespace SoftwareDeveloperCase.Infrastructure.Services;

/// <summary>
/// Implementation of password service using BCrypt for hashing and verification
/// </summary>
public class PasswordService : IPasswordService
{
    private const int WorkFactor = 12;

    /// <summary>
    /// Hashes a plain text password using BCrypt with work factor 12
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>The hashed password</returns>
    /// <exception cref="ArgumentException">Thrown when password is null or empty</exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        return BC.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a plain text password against a hashed password
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The hashed password to verify against</param>
    /// <returns>True if the password matches, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when either parameter is null or empty</exception>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));
        }

        try
        {
            return BC.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            // If hash is in invalid format, return false
            return false;
        }
    }

    /// <summary>
    /// Checks if a password needs to be rehashed (e.g., work factor changed)
    /// </summary>
    /// <param name="hashedPassword">The hashed password to check</param>
    /// <returns>True if the password needs rehashing, false otherwise</returns>
    public bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return true;
        }

        try
        {
            // If the hash doesn't start with $2a$, $2b$, or $2y$, it's not a BCrypt hash
            if (!hashedPassword.StartsWith("$2a$") && !hashedPassword.StartsWith("$2b$") && !hashedPassword.StartsWith("$2y$"))
            {
                return true;
            }

            // Extract work factor from hash
            var parts = hashedPassword.Split('$');
            if (parts.Length < 4 || !int.TryParse(parts[2], out int hashWorkFactor))
            {
                return true;
            }

            return hashWorkFactor < WorkFactor;
        }
        catch (Exception)
        {
            return true;
        }
    }
}