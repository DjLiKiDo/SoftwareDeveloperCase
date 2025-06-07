using System.Collections;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern that coordinates the work of multiple repositories
/// </summary>
internal class UnitOfWork : IUnitOfWork
{
    private Hashtable? _repositories;
    private readonly SoftwareDeveloperCaseDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _currentTransaction;

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
    /// Gets a value indicating whether a transaction is currently active
    /// </summary>
    public bool HasActiveTransaction => _currentTransaction != null;

    /// <summary>
    /// Gets the current transaction identifier, or null if no transaction is active
    /// </summary>
    public Guid? CurrentTransactionId => _currentTransaction?.TransactionId;

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

    #region Transaction Management

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Transaction already active. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);
            throw new InvalidOperationException("A transaction is already active. Only one transaction is supported at a time.");
        }

        _logger.LogDebug("Beginning new database transaction");
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("Database transaction started. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);
    }

    /// <summary>
    /// Commits the current transaction and saves all changes
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The number of affected rows</returns>
    public async Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("Attempted to commit transaction but no active transaction found");
            throw new InvalidOperationException("No active transaction to commit");
        }

        try
        {
            _logger.LogDebug("Committing transaction. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);

            var result = await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Transaction committed successfully. Transaction ID: {TransactionId}, Affected rows: {AffectedRows}",
                _currentTransaction.TransactionId, result);

            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;

            return result;
        }
        catch (Exception ex)
        {
            var transactionId = _currentTransaction?.TransactionId;
            _logger.LogError(ex, "Error occurred while committing transaction. Transaction ID: {TransactionId}", transactionId);
            await RollbackTransactionAsync(cancellationToken);
            throw new ApplicationException("An error occurred while committing the transaction", ex);
        }
    }

    /// <summary>
    /// Rolls back the current transaction and discards all changes
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("Attempted to rollback transaction but no active transaction found");
            return;
        }

        try
        {
            _logger.LogDebug("Rolling back transaction. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);
            await _currentTransaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Transaction rolled back successfully. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);

            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
        catch (Exception ex)
        {
            var transactionId = _currentTransaction?.TransactionId;
            _logger.LogError(ex, "Error occurred while rolling back transaction. Transaction ID: {TransactionId}", transactionId);

            // If rollback fails, dispose the transaction to clean up
            try
            {
                await _currentTransaction!.DisposeAsync();
            }
            catch
            {
                // Ignore dispose errors
            }
            finally
            {
                _currentTransaction = null;
            }

            throw new ApplicationException("An error occurred while rolling back the transaction", ex);
        }
    }

    /// <summary>
    /// Executes a function within a database transaction with automatic rollback on exceptions
    /// </summary>
    /// <typeparam name="T">The return type of the function</typeparam>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>The result of the operation</returns>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        var wasTransactionStartedHere = !HasActiveTransaction;

        try
        {
            if (wasTransactionStartedHere)
            {
                await BeginTransactionAsync(cancellationToken);
            }

            _logger.LogDebug("Executing operation within transaction. Transaction ID: {TransactionId}", CurrentTransactionId);
            var result = await operation();

            if (wasTransactionStartedHere)
            {
                await CommitTransactionAsync(cancellationToken);
                _logger.LogDebug("Operation completed and transaction committed successfully. Transaction ID: {TransactionId}", CurrentTransactionId);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (wasTransactionStartedHere)
            {
                _logger.LogError(ex, "Operation failed, rolling back transaction. Transaction ID: {TransactionId}", CurrentTransactionId);
                await RollbackTransactionAsync(cancellationToken);
            }
            throw;
        }
    }

    /// <summary>
    /// Executes an action within a database transaction with automatic rollback on exceptions
    /// </summary>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        var wasTransactionStartedHere = !HasActiveTransaction;

        try
        {
            if (wasTransactionStartedHere)
            {
                await BeginTransactionAsync(cancellationToken);
            }

            _logger.LogDebug("Executing operation within transaction. Transaction ID: {TransactionId}", CurrentTransactionId);
            await operation();

            if (wasTransactionStartedHere)
            {
                await CommitTransactionAsync(cancellationToken);
                _logger.LogDebug("Operation completed and transaction committed successfully. Transaction ID: {TransactionId}", CurrentTransactionId);
            }
        }
        catch (Exception ex)
        {
            if (wasTransactionStartedHere)
            {
                _logger.LogError(ex, "Operation failed, rolling back transaction. Transaction ID: {TransactionId}", CurrentTransactionId);
                await RollbackTransactionAsync(cancellationToken);
            }
            throw;
        }
    }

    #endregion

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

    /// <summary>
    /// Disposes the context when the unit of work is disposed
    /// </summary>
    public void Dispose()
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Disposing UnitOfWork with active transaction. Rolling back transaction. Transaction ID: {TransactionId}", _currentTransaction.TransactionId);
            _currentTransaction.Rollback();
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        _context.Dispose();
    }
}
