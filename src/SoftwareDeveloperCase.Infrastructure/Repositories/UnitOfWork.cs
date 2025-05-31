using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using System.Collections;

namespace SoftwareDeveloperCase.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private Hashtable? _repositories;

    private readonly SoftwareDeveloperCaseDbContext _context;

    private IDepartmentRepository? _departmentRepository;
    private IPermissionRepository? _permissionRepository;
    private IRolePermissionRepository? _rolePermissionRepository;
    private IRoleRepository? _roleRepository;
    private IUserRepository? _userRepository;
    private IUserRoleRepository? _userRoleRepository;

    // Core domain repositories
    private ITeamRepository? _teamRepository;
    private ITeamMemberRepository? _teamMemberRepository;
    private IProjectRepository? _projectRepository;
    private ITaskRepository? _taskRepository;
    private ITaskCommentRepository? _taskCommentRepository;

    public IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository(_context);
    public IPermissionRepository PermissionRepository => _permissionRepository ??= new PermissionRepository(_context);
    public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository ??= new RolePermissionRepository(_context);
    public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
    public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_context);

    // Core domain repositories
    public ITeamRepository TeamRepository => _teamRepository ??= new TeamRepository(_context);
    public ITeamMemberRepository TeamMemberRepository => _teamMemberRepository ??= new TeamMemberRepository(_context);
    public IProjectRepository ProjectRepository => _projectRepository ??= new ProjectRepository(_context);
    public ITaskRepository TaskRepository => _taskRepository ??= new TaskRepository(_context);
    public ITaskCommentRepository TaskCommentRepository => _taskCommentRepository ??= new TaskCommentRepository(_context);

    public SoftwareDeveloperCaseDbContext SoftwareDeveloperCaseDbContext => _context;

    public UnitOfWork(SoftwareDeveloperCaseDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChanges()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // TODO: Log error
            throw new Exception($"Unit Of Work error --> {ex.Message}");
        }
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Log error
            throw new Exception($"Unit Of Work error --> {ex.Message}");
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

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

        return (IRepository<TEntity>)_repositories[type]!;
    }
}
