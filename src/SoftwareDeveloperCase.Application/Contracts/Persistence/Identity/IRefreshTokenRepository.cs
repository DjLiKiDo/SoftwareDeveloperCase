using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;

/// <summary>
/// Repository interface for RefreshToken entity operations
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Gets an active refresh token by token value
    /// </summary>
    /// <param name="token">The refresh token value</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The refresh token if found and active, null otherwise</returns>
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The list of active refresh tokens</returns>
    Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
