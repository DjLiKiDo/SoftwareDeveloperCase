using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Team repository implementation providing data access for Team entities
/// </summary>
internal class TeamRepository : Repository<Team>, ITeamRepository
{
    public TeamRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets teams by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teams the user belongs to</returns>
    public async Task<IReadOnlyList<Team>> GetTeamsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var teams = await (_context.Teams!
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User)
            .Where(t => t.TeamMembers.Any(m => m.UserId == userId)) // No longer need int conversion
            .ToListAsync(cancellationToken));
        
        return teams.AsReadOnly();
    }

    /// <summary>
    /// Gets teams by team leader ID
    /// </summary>
    /// <param name="leaderId">The team leader ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teams led by the user</returns>
    public async Task<IReadOnlyList<Team>> GetTeamsByLeaderIdAsync(Guid leaderId, CancellationToken cancellationToken = default)
    {
        var teams = await (_context.Teams!
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User)
            .Where(t => t.TeamMembers.Any(m => m.UserId == leaderId && m.TeamRole == TeamRole.Leader)) // No longer need int conversion
            .ToListAsync(cancellationToken));
        
        return teams.AsReadOnly();
    }

    /// <summary>
    /// Gets active teams with their project count
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active teams</returns>
    public async Task<IReadOnlyList<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await (_context.Teams!
            .Include(t => t.TeamMembers.Where(m => m.Status == MemberStatus.Active))
            .ThenInclude(tm => tm.User)
            .Include(t => t.Projects)
            .ToListAsync(cancellationToken));
        
        return teams.AsReadOnly();
    }

    /// <summary>
    /// Gets team with members included
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team with members or null if not found</returns>
    public async Task<Team?> GetTeamWithMembersAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return await (_context.Teams!
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User))
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
    }

    /// <summary>
    /// Checks if team name exists
    /// </summary>
    /// <param name="name">The team name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    public async Task<bool> IsTeamNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await (_context.Teams!
            .AnyAsync(t => t.Name.ToLower() == name.ToLower() && (excludeId == null || t.Id != excludeId), cancellationToken));
    }

    /// <summary>
    /// Checks if a user exists in any team
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user is a team member</returns>
    public async Task<bool> IsUserInAnyTeamAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await (_context.TeamMembers! // Changed to _context.TeamMembers
            .AnyAsync(tm => tm.UserId == userId && tm.Status == MemberStatus.Active, cancellationToken));
    }

    /// <summary>
    /// Gets teams with active projects count
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Teams with project statistics</returns>
    public async Task<IReadOnlyList<Team>> GetTeamsWithActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await _context.Teams!
            .Include(t => t.Projects.Where(p => p.Status == ProjectStatus.Active))
            .ToListAsync(cancellationToken);
        
        return teams.AsReadOnly();
    }

    /// <summary>
    /// Gets teams with their projects
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Teams with projects</returns>
    public async Task<IEnumerable<Team>> GetTeamsWithProjectsAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.Teams?.Include(t => t.Projects);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<Team>();
    }
}
