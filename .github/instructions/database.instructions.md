# Database Instructions

## SoftwareDeveloperCase - Entity Framework & Database Guidelines

### Context

This file provides specific instructions for database operations in the SoftwareDeveloperCase project using Entity Framework Core with SQL Server, following Clean Architecture principles.

---

## Entity Configuration

### Base Entity Pattern

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    protected BaseEntity() { }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Entity Configuration Classes

```csharp
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Description)
               .HasMaxLength(2000);

        builder.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(p => p.DueDate)
               .HasColumnType("datetime2");

        // Value Objects
        builder.OwnsOne(p => p.Budget, budget =>
        {
            budget.Property(b => b.Amount)
                  .HasPrecision(18, 2)
                  .HasColumnName("BudgetAmount");

            budget.Property(b => b.Currency)
                  .HasMaxLength(3)
                  .HasColumnName("BudgetCurrency");
        });

        // Relationships
        builder.HasMany(p => p.Tasks)
               .WithOne(t => t.Project)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.TeamMembers)
               .WithOne(tm => tm.Project)
               .HasForeignKey(tm => tm.ProjectId);

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);

        // Query Filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);

        // Ignore navigation properties that shouldn't be mapped
        builder.Ignore(p => p.DomainEvents);
    }
}
```

---

## Repository Pattern Implementation

### Base Repository Interface

```csharp
public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Repository Implementation

```csharp
public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.TeamMembers)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Project>> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) ||
                                   p.Description.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Project>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
    }

    public void Update(Project project)
    {
        project.MarkAsUpdated();
        _context.Projects.Update(project);
    }

    public void Delete(Project project)
    {
        project.MarkAsDeleted();
        _context.Projects.Update(project); // Soft delete
    }
}
```

---

## Unit of Work Pattern

### Unit of Work Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    IProjectRepository Projects { get; }
    ITaskRepository Tasks { get; }
    IUserRepository Users { get; }
    ITeamRepository Teams { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### Unit of Work Implementation

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Projects = new ProjectRepository(_context);
        Tasks = new TaskRepository(_context);
        Users = new UserRepository(_context);
        Teams = new TeamRepository(_context);
    }

    public IProjectRepository Projects { get; }
    public ITaskRepository Tasks { get; }
    public IUserRepository Users { get; }
    public ITeamRepository Teams { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle domain events before saving
        await DispatchDomainEventsAsync();

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        var entities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity);

        var domainEvents = entities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        entities.ToList().ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

---

## Database Context Configuration

### DbContext Setup

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType);
                var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo!, parameter, Expression.Constant("IsDeleted"));
                var compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.MarkAsUpdated();
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

---

## Migration Patterns

### Migration Naming Convention

```
YYYYMMDD_HHMMSS_DescriptiveName
```

### Example Migration Commands

```bash
# Add new migration
dotnet ef migrations add 20250601_120000_AddProjectEntity -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api

# Update database
dotnet ef database update -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api

# Generate script
dotnet ef migrations script -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api

# Remove last migration (if not applied)
dotnet ef migrations remove -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api
```

### Migration Best Practices

```csharp
public partial class AddProjectEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Always check for existing objects before creating
        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                BudgetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                BudgetCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.Id);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_Projects_Name",
            table: "Projects",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_Status",
            table: "Projects",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_CreatedAt",
            table: "Projects",
            column: "CreatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Projects");
    }
}
```

---

## Query Optimization

### Efficient Queries

```csharp
// Good: Use projection for list views
public async Task<IEnumerable<ProjectSummaryDto>> GetProjectSummariesAsync()
{
    return await _context.Projects
        .Select(p => new ProjectSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Status = p.Status,
            TaskCount = p.Tasks.Count(),
            CompletedTaskCount = p.Tasks.Count(t => t.Status == TaskStatus.Done)
        })
        .ToListAsync();
}

// Good: Use Include for related data when needed
public async Task<Project?> GetProjectWithTasksAsync(Guid id)
{
    return await _context.Projects
        .Include(p => p.Tasks.Where(t => !t.IsDeleted))
        .FirstOrDefaultAsync(p => p.Id == id);
}

// Good: Use pagination for large datasets
public async Task<PagedResult<Project>> GetProjectsAsync(int page, int pageSize)
{
    var query = _context.Projects.AsQueryable();

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Project>(items, totalCount, page, pageSize);
}
```

### Query Performance Tips

```csharp
// Use AsNoTracking for read-only operations
public async Task<IEnumerable<ProjectDto>> GetAllProjectsReadOnlyAsync()
{
    return await _context.Projects
        .AsNoTracking()
        .Select(p => new ProjectDto { /* ... */ })
        .ToListAsync();
}

// Use compiled queries for frequently used queries
private static readonly Func<ApplicationDbContext, Guid, Task<Project?>> GetProjectByIdCompiled =
    EF.CompileAsyncQuery((ApplicationDbContext context, Guid id) =>
        context.Projects.FirstOrDefault(p => p.Id == id));

public async Task<Project?> GetProjectByIdAsync(Guid id)
{
    return await GetProjectByIdCompiled(_context, id);
}

// Use Split Query for multiple includes
public async Task<Project?> GetProjectWithAllRelatedDataAsync(Guid id)
{
    return await _context.Projects
        .AsSplitQuery()
        .Include(p => p.Tasks)
        .Include(p => p.TeamMembers)
        .ThenInclude(tm => tm.User)
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

---

## Connection String Configuration

### Development Environment

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SoftwareDeveloperCase_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### Production Environment

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true;Connection Timeout=30"
  }
}
```

### DbContext Registration

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SoftwareDeveloperCase.Infrastructure"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

---

## Database Seeding

### Seed Data Configuration

```csharp
public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        // Seed roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { Name = "Admin", Description = "Administrator role" },
                new Role { Name = "Manager", Description = "Project manager role" },
                new Role { Name = "Developer", Description = "Developer role" }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Seed admin user
        if (!await context.Users.AnyAsync())
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var adminUser = new User
            {
                UserName = "admin@softwarecorp.com",
                Email = "admin@softwarecorp.com",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
```

---

## Error Handling

### Database Exceptions

```csharp
public class DatabaseExceptionHandler
{
    public static (ErrorType Type, string Message) HandleException(Exception exception)
    {
        return exception switch
        {
            DbUpdateConcurrencyException => (ErrorType.Conflict, "The record was modified by another user."),
            DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx => sqlEx.Number switch
            {
                2 => (ErrorType.Infrastructure, "Database connection timeout."),
                547 => (ErrorType.Conflict, "Cannot delete record due to related data."),
                2601 or 2627 => (ErrorType.Conflict, "A record with this value already exists."),
                _ => (ErrorType.Infrastructure, "A database error occurred.")
            },
            InvalidOperationException => (ErrorType.Validation, "Invalid operation on the database."),
            _ => (ErrorType.Infrastructure, "An unexpected database error occurred.")
        };
    }
}
```

---

## Testing Database Operations

### In-Memory Database for Testing

```csharp
public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
```

### Repository Testing

```csharp
public class ProjectRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProjectRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldAddProjectToDatabase()
    {
        // Arrange
        await using var context = _fixture.CreateContext();
        var repository = new ProjectRepository(context);
        var project = Project.Create("Test Project", "Description", DateTime.Now.AddDays(30)).Value;

        // Act
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Assert
        var savedProject = await context.Projects.FindAsync(project.Id);
        savedProject.Should().NotBeNull();
        savedProject.Name.Should().Be("Test Project");
    }
}
```

---

## Performance Monitoring

### Database Monitoring

```csharp
// Add to Program.cs for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString)
               .LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors();
    });
}
```

### Query Logging

```csharp
public class QueryLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<QueryLoggingInterceptor> _logger;

    public QueryLoggingInterceptor(ILogger<QueryLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Duration.TotalMilliseconds > 1000) // Log slow queries
        {
            _logger.LogWarning("Slow query detected: {Query} took {Duration}ms",
                command.CommandText, eventData.Duration.TotalMilliseconds);
        }

        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
}
```

---

## Best Practices Summary

1. **Always use async/await** for database operations
2. **Include CancellationToken** in all async methods
3. **Use Repository and Unit of Work patterns** for data access
4. **Configure entities using IEntityTypeConfiguration**
5. **Implement soft delete** with query filters
6. **Use migrations** for schema changes
7. **Apply proper indexing** for performance
8. **Use projections** for read-only operations
9. **Implement proper error handling** for database exceptions
10. **Use transactions** for multi-step operations
11. **Test repository operations** with in-memory database
12. **Monitor query performance** in development and production
