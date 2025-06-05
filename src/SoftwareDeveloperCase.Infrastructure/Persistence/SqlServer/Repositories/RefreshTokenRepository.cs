using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Entities.Identity;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

internal class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets an active refresh token by token value
    /// </summary>
    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.RefreshTokens?
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > now, cancellationToken) ?? null;
    }

    /// <summary>
    /// Gets all active refresh tokens for a user
    /// </summary>
    public async Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.RefreshTokens?
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > now)
            .ToListAsync(cancellationToken) ?? new List<RefreshToken>();
    }

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var activeTokens = await _context.RefreshTokens?
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > now)
            .ToListAsync(cancellationToken) ?? new List<RefreshToken>();

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        if (activeTokens.Any())
        {
            _context.RefreshTokens?.UpdateRange(activeTokens);
        }
    }
}
