using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Task;
using SoftwareDeveloperCase.Domain.Entities.Team;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Configurations;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

/// <summary>
/// Entity Framework DbContext for the SoftwareDeveloperCase application
/// </summary>
public class SoftwareDeveloperCaseDbContext : DbContext
{
    private readonly IDateTimeService _dateTimeService;
    private readonly EntitySaveChangesInterceptor _entitySaveChangesInterceptor;

    /// <summary>
    /// Gets or sets the Users database set
    /// </summary>
    public DbSet<User>? Users { get; set; }

    /// <summary>
    /// Gets or sets the Roles database set
    /// </summary>
    public DbSet<Role>? Roles { get; set; }

    /// <summary>
    /// Gets or sets the Permissions database set
    /// </summary>
    public DbSet<Permission>? Permissions { get; set; }

    /// <summary>
    /// Gets or sets the UserRoles database set
    /// </summary>
    public DbSet<UserRole>? UserRoles { get; set; }

    /// <summary>
    /// Gets or sets the RefreshTokens database set
    /// </summary>
    public DbSet<RefreshToken>? RefreshTokens { get; set; }

    // Core Domain Entities

    /// <summary>
    /// Gets or sets the Teams database set
    /// </summary>
    public DbSet<Team>? Teams { get; set; }

    /// <summary>
    /// Gets or sets the TeamMembers database set
    /// </summary>
    public DbSet<TeamMember>? TeamMembers { get; set; }

    /// <summary>
    /// Gets or sets the Projects database set
    /// </summary>
    public DbSet<Project>? Projects { get; set; }

    /// <summary>
    /// Gets or sets the Tasks database set
    /// </summary>
    public DbSet<Domain.Entities.Task.Task>? Tasks { get; set; }

    /// <summary>
    /// Gets or sets the TaskComments database set
    /// </summary>
    public DbSet<TaskComment>? TaskComments { get; set; }

    /// <summary>
    /// Initializes a new instance of the SoftwareDeveloperCaseDbContext class (for design-time operations)
    /// </summary>
    public SoftwareDeveloperCaseDbContext() : this(new DbContextOptions<SoftwareDeveloperCaseDbContext>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the SoftwareDeveloperCaseDbContext class (for design-time operations)
    /// </summary>
    /// <param name="options">The database context options</param>
    public SoftwareDeveloperCaseDbContext(DbContextOptions<SoftwareDeveloperCaseDbContext> options)
        : base(options)
    {
        // Create default implementations for design-time operations
        _dateTimeService = new DesignTimeService();
        _entitySaveChangesInterceptor = new EntitySaveChangesInterceptor(_dateTimeService);
    }

    /// <summary>
    /// Initializes a new instance of the SoftwareDeveloperCaseDbContext class
    /// </summary>
    /// <param name="options">The database context options</param>
    /// <param name="entitySaveChangesInterceptor">The save changes interceptor</param>
    /// <param name="dateTimeService">The date time service</param>
    public SoftwareDeveloperCaseDbContext(DbContextOptions<SoftwareDeveloperCaseDbContext> options, EntitySaveChangesInterceptor entitySaveChangesInterceptor, IDateTimeService dateTimeService)
        : base(options)
    {
        _dateTimeService = dateTimeService;
        _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
    }

    /// <summary>
    /// Simple implementation of IDateTimeService for design-time operations
    /// </summary>
    private class DesignTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }

    /// <summary>
    /// Configures the model relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new TaskCommentConfiguration());

        modelBuilder.UseSingularTableNameConvention();

        modelBuilder.SeedDataBase(_dateTimeService);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the database context with interceptors and other configuration options.
    /// </summary>
    /// <param name="optionsBuilder">The options builder used to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure database provider for design-time if not already configured
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SoftwareDeveloperCaseDb;Trusted_Connection=true;MultipleActiveResultSets=true");
        }

        optionsBuilder.AddInterceptors(_entitySaveChangesInterceptor);

        base.OnConfiguring(optionsBuilder);
    }
}
