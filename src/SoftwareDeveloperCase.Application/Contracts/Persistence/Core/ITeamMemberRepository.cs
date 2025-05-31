using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Domain.Entities.Core;

namespace SoftwareDeveloperCase.Application.Contracts.Persistence.Core;

/// <summary>
/// Repository interface for TeamMember entity operations
/// </summary>
public interface ITeamMemberRepository : IRepository<TeamMember>
{
    /// <summary>
    /// Gets team members by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of team members</returns>
    Task<IReadOnlyList<TeamMember>> GetMembersByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of team memberships</returns>
    Task<IReadOnlyList<TeamMember>> GetTeamsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team member by team and user IDs
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team member or null if not found</returns>
    Task<TeamMember?> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user is member of team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user is member, false otherwise</returns>
    Task<bool> IsUserTeamMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active team members by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active team members</returns>
    Task<IReadOnlyList<TeamMember>> GetActiveMembersByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
}
