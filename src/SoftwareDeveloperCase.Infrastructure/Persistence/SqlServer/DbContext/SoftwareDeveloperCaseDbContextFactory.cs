using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SoftwareDeveloperCase.Application.Contracts.Services;

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

/// <summary>
/// Design-time factory for creating SoftwareDeveloperCaseDbContext instances
/// Used by Entity Framework tools for migrations and other design-time operations
/// </summary>
public class SoftwareDeveloperCaseDbContextFactory : IDesignTimeDbContextFactory<SoftwareDeveloperCaseDbContext>
{
    /// <summary>
    /// Creates a DbContext instance for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A configured DbContext instance</returns>
    public SoftwareDeveloperCaseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SoftwareDeveloperCaseDbContext>();

        // Configure database provider for design-time
        // Use SQL Server for migrations even if in-memory is configured for development
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=SoftwareDeveloperCaseDb;Trusted_Connection=true;MultipleActiveResultSets=true";
        optionsBuilder.UseSqlServer(connectionString);

        // Create design-time implementations of dependencies
        var dateTimeService = new DesignTimeService();
        var saveChangesInterceptor = new EntitySaveChangesInterceptor(dateTimeService);

        return new SoftwareDeveloperCaseDbContext(optionsBuilder.Options, saveChangesInterceptor, dateTimeService);
    }

    /// <summary>
    /// Simple implementation of IDateTimeService for design-time operations
    /// </summary>
    private class DesignTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
