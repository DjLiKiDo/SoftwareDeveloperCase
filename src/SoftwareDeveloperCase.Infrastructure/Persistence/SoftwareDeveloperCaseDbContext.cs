using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.Entities.Lookups;
using SoftwareDeveloperCase.Domain.ValueObjects;
using SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence;

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
    /// Gets or sets the Departments database set
    /// </summary>
    public DbSet<Department>? Departments { get; set; }

    /// <summary>
    /// Gets or sets the UserRoles database set
    /// </summary>
    public DbSet<UserRole>? UserRoles { get; set; }

    // Core Domain Entities

    /// <summary>
    /// Gets or sets the Teams database set
    /// </summary>
    public DbSet<Team>? Teams { get; set; }

    /// <summary>
    /// Gets or sets the TeamMembers database set
    /// </summary>
    public DbSet<Domain.Entities.Core.TeamMember>? TeamMembers { get; set; }

    /// <summary>
    /// Gets or sets the Projects database set
    /// </summary>
    public DbSet<Project>? Projects { get; set; }

    /// <summary>
    /// Gets or sets the Tasks database set
    /// </summary>
    public DbSet<Domain.Entities.Core.Task>? Tasks { get; set; }

    /// <summary>
    /// Gets or sets the TaskComments database set
    /// </summary>
    public DbSet<TaskComment>? TaskComments { get; set; }

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
    /// Configures the model relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Email value object conversion
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(255);

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
        optionsBuilder.AddInterceptors(_entitySaveChangesInterceptor);

        base.OnConfiguring(optionsBuilder);
    }
}
