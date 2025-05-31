using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;
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
        // Configure Email value object conversion
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(255);

        // Configure TaskHierarchy value object as owned entity
        modelBuilder.Entity<Domain.Entities.Core.Task>()
            .OwnsOne(t => t.Hierarchy, hierarchy =>
            {
                hierarchy.Property(h => h.Level)
                    .HasColumnName("HierarchyLevel")
                    .IsRequired();
                
                hierarchy.Property(h => h.Path)
                    .HasColumnName("HierarchyPath")
                    .HasMaxLength(500)
                    .IsRequired();
                
                hierarchy.Property(h => h.Order)
                    .HasColumnName("HierarchyOrder")
                    .IsRequired();
            });

        // Configure Task decimal properties with precision and scale
        modelBuilder.Entity<Domain.Entities.Core.Task>()
            .Property(t => t.EstimatedHours)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Domain.Entities.Core.Task>()
            .Property(t => t.ActualHours)
            .HasPrecision(18, 2);

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
