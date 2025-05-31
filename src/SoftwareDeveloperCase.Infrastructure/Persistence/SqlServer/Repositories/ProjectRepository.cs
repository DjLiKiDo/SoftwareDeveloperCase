using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.Enums.Core;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Project repository implementation providing data access for Project entities
/// </summary>
internal class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(SoftwareDeveloperCaseDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets projects by status
    /// </summary>
    /// <param name="status">Project status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects with specified status</returns>
    public async Task<IReadOnlyList<Project>> GetProjectsByStatusAsync(Domain.Enums.Core.ProjectStatus status, CancellationToken cancellationToken = default)
    {
        var projects = await _context.Projects!
            .Include(p => p.Team)
            .Include(p => p.Tasks)
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
        return projects.AsReadOnly();
    }

    /// <summary>
    /// Gets projects by team ID
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects</returns>
    public async Task<IReadOnlyList<Project>> GetProjectsByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects?.Where(p => p.TeamId == teamId)
                                      .Include(p => p.Team!);
        
        return query != null ? await query.ToListAsync(cancellationToken) : new List<Project>();
    }

    /// <summary>
    /// Gets projects by user ID (where user is assigned or team member)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of projects the user has access to</returns>
    public async Task<IReadOnlyList<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var projects = await _context.Projects!
            .Include(p => p.Team)
                .ThenInclude(t => t!.TeamMembers) // Changed from Members to TeamMembers
            .Include(p => p.Tasks)
            .Where(p => p.Team!.TeamMembers.Any(tm => tm.UserId == userId) || // Changed from Members to TeamMembers
                       p.Tasks.Any(t => t.AssignedToId == userId)) // Changed to AssignedToId
            .ToListAsync(cancellationToken);
        return projects.AsReadOnly();
    }

    /// <summary>
    /// Gets project with team and tasks included
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project with related data or null if not found</returns>
    public async Task<Project?> GetProjectWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects!
            .Include(p => p.Team)
            .ThenInclude(t => t!.TeamMembers) // Changed from Members to TeamMembers
            .ThenInclude(tm => tm.User)
            .Include(p => p.Tasks)
            .ThenInclude(t => t.AssignedTo) // Changed to AssignedTo
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Comments)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    /// <summary>
    /// Checks if project name exists within a team
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="name">Project name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    public async Task<bool> IsProjectNameExistsInTeamAsync(Guid teamId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects!
            .Where(p => p.TeamId == teamId && p.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Gets active projects by team ID
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active projects</returns>
    public async Task<IReadOnlyList<Project>> GetActiveProjectsByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var projects = await _context.Projects!
            .Include(p => p.Team)
            .Include(p => p.Tasks)
            .Where(p => p.TeamId == teamId && p.Status == Domain.Enums.Core.ProjectStatus.Active)
            .ToListAsync(cancellationToken);
        return projects.AsReadOnly();
    }
}
