using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Team member repository implementation providing data access for TeamMember entities
/// </summary>
internal class TeamMemberRepository : Repository<TeamMember>, ITeamMemberRepository
{
    public TeamMemberRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets team members by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of team members</returns>
    public async Task<IReadOnlyList<TeamMember>> GetMembersByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var members = await _context.TeamMembers!
            .Include(tm => tm.User)
            .Include(tm => tm.Team)
            .Where(tm => tm.TeamId == teamId)
            .ToListAsync(cancellationToken);
        
        return members.AsReadOnly();
    }

    /// <summary>
    /// Gets teams by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of team memberships</returns>
    public async Task<IReadOnlyList<TeamMember>> GetTeamsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var members = await _context.TeamMembers!
            .Include(tm => tm.Team)
            .Include(tm => tm.User)
            .Where(tm => tm.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return members.AsReadOnly();
    }

    /// <summary>
    /// Gets team member by team and user IDs
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team member or null if not found</returns>
    public async Task<TeamMember?> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TeamMembers!
            .Include(tm => tm.User)
            .Include(tm => tm.Team)
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// Checks if user is member of team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user is member, false otherwise</returns>
    public async Task<bool> IsUserTeamMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TeamMembers!
            .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// Gets active team members by team ID
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active team members</returns>
    public async Task<IReadOnlyList<TeamMember>> GetActiveMembersByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var members = await _context.TeamMembers!
            .Include(tm => tm.User)
            .Include(tm => tm.Team)
            .Where(tm => tm.TeamId == teamId && tm.Status == MemberStatus.Active)
            .ToListAsync(cancellationToken);
        
        return members.AsReadOnly();
    }

    public async Task<IEnumerable<TeamMember>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var query = _context.TeamMembers?.Where(tm => tm.TeamId == teamId)
                                         .Include(tm => tm.User);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TeamMember>();
    }

    public async Task<IEnumerable<TeamMember>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = _context.TeamMembers?.Where(tm => tm.UserId == userId)
                                         .Include(tm => tm.Team);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TeamMember>();
    }

    public async Task<IEnumerable<TeamMember>> GetActiveTeamMembersAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var query = _context.TeamMembers?.Where(tm => tm.TeamId == teamId && tm.Status == MemberStatus.Active)
                                         .Include(tm => tm.User);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TeamMember>();
    }

    public async Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default)
    {
        var query = _context.TeamMembers?.Where(tm => tm.UserId == userId && tm.TeamId == teamId);
        return query != null && await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeamMember>> GetTeamLeadersAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var query = _context.TeamMembers?.Where(tm => tm.TeamId == teamId && tm.TeamRole == TeamRole.Leader) // Changed from tm.Role to tm.TeamRole
                                         .Include(tm => tm.User);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<TeamMember>();
    }
}
