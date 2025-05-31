using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern that coordinates the work of multiple repositories
/// </summary>
internal class UnitOfWork : IUnitOfWork
{
    private Hashtable? _repositories;
    private readonly SoftwareDeveloperCaseDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    // Identity repositories
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    // Core domain repositories
    private readonly ITeamRepository _teamRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskCommentRepository _taskCommentRepository;

    /// <summary>
    /// Gets the permission repository
    /// </summary>
    public IPermissionRepository PermissionRepository => _permissionRepository;

    /// <summary>
    /// Gets the role permission repository
    /// </summary>
    public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository;

    /// <summary>
    /// Gets the role repository
    /// </summary>
    public IRoleRepository RoleRepository => _roleRepository;

    /// <summary>
    /// Gets the user repository
    /// </summary>
    public IUserRepository UserRepository => _userRepository;

    /// <summary>
    /// Gets the user role repository
    /// </summary>
    public IUserRoleRepository UserRoleRepository => _userRoleRepository;

    /// <summary>
    /// Gets the team repository
    /// </summary>
    public ITeamRepository TeamRepository => _teamRepository;

    /// <summary>
    /// Gets the team member repository
    /// </summary>
    public ITeamMemberRepository TeamMemberRepository => _teamMemberRepository;

    /// <summary>
    /// Gets the project repository
    /// </summary>
    public IProjectRepository ProjectRepository => _projectRepository;

    /// <summary>
    /// Gets the task repository
    /// </summary>
    public ITaskRepository TaskRepository => _taskRepository;

    /// <summary>
    /// Gets the task comment repository
    /// </summary>
    public ITaskCommentRepository TaskCommentRepository => _taskCommentRepository;

    /// <summary>
    /// Gets the database context
    /// </summary>
    public SoftwareDeveloperCaseDbContext SoftwareDeveloperCaseDbContext => _context;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class with all required repositories
    /// </summary>
    public UnitOfWork(
        SoftwareDeveloperCaseDbContext context,
        ILogger<UnitOfWork> logger,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        ITeamRepository teamRepository,
        ITeamMemberRepository teamMemberRepository,
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ITaskCommentRepository taskCommentRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Identity repositories
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _rolePermissionRepository = rolePermissionRepository ?? throw new ArgumentNullException(nameof(rolePermissionRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));

        // Core domain repositories
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _teamMemberRepository = teamMemberRepository ?? throw new ArgumentNullException(nameof(teamMemberRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _taskCommentRepository = taskCommentRepository ?? throw new ArgumentNullException(nameof(taskCommentRepository));
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The number of state entries written to the database</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database asynchronously");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving changes to database");
            throw new ApplicationException("An error occurred while saving changes to the database", ex);
        }
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    public async Task<int> SaveChanges()
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving changes to database");
            throw new ApplicationException("An error occurred while saving changes to the database", ex);
        }
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the database synchronously
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    public int SaveChangesSynchronously()
    {
        try
        {
            _logger.LogDebug("Saving changes to database synchronously");
            return _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving changes to database");
            throw new ApplicationException("An error occurred while saving changes to the database", ex);
        }
    }

    /// <summary>
    /// Disposes the context when the unit of work is disposed
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Retrieves a generic repository for the specified entity type
    /// </summary>
    /// <typeparam name="TEntity">The entity type that extends BaseEntity</typeparam>
    /// <returns>The repository instance for the entity type</returns>
    public IRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity
    {
        _repositories ??= new Hashtable();

        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>?)_repositories[type];
    }
}
