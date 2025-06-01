using SoftwareDeveloperCase.Domain.Entities.Team;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Core;

/// <summary>
/// Repository interface for Team entity operations
/// </summary>
public interface ITeamRepository : IRepository<Team>
{
    /// <summary>
    /// Gets teams by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teams the user belongs to</returns>
    Task<IReadOnlyList<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams by team leader ID
    /// </summary>
    /// <param name="leaderId">The team leader ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teams led by the user</returns>
    Task<IReadOnlyList<Team>> GetTeamsByLeaderIdAsync(Guid leaderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team with members included
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team with members or null if not found</returns>
    Task<Team?> GetTeamWithMembersAsync(Guid teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if team name exists
    /// </summary>
    /// <param name="name">The team name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> IsTeamNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
