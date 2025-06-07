using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using SoftwareDeveloperCase.Test.Integration.Common;
using Xunit;

namespace SoftwareDeveloperCase.Test.Integration.Persistence;

/// <summary>
/// Base class for integration tests that require database access with proper cleanup
/// </summary>
public abstract class DatabaseIntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly SoftwareDeveloperCaseDbContext Context;

    protected DatabaseIntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Scope = Factory.Services.CreateScope();
        UnitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Context = Scope.ServiceProvider.GetRequiredService<SoftwareDeveloperCaseDbContext>();

        // Ensure we start with a clean slate for each test
        CleanupDatabase().Wait();
    }

    /// <summary>
    /// Cleans up test data from the database
    /// </summary>
    protected virtual async Task CleanupDatabase()
    {
        try
        {
            // Clean up test data in dependency order to avoid foreign key violations
            // Use raw SQL for better EF Core compatibility with complex queries

            // First clean up dependent entities
            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE ur FROM UserRole ur
                INNER JOIN [User] u ON ur.UserId = u.Id
                WHERE u.Email LIKE '%test%' OR u.Email LIKE '%@example.com%'");

            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE tm FROM TeamMember tm
                INNER JOIN [User] u ON tm.UserId = u.Id
                WHERE u.Email LIKE '%test%' OR u.Email LIKE '%@example.com%'");

            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE tc FROM TaskComment tc
                INNER JOIN [User] u ON tc.AuthorId = u.Id
                WHERE u.Email LIKE '%test%' OR u.Email LIKE '%@example.com%'");

            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE t FROM Task t
                INNER JOIN [User] u ON t.AssignedToId = u.Id
                WHERE u.Email LIKE '%test%' OR u.Email LIKE '%@example.com%'");

            // Then clean up main entities
            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM [User] WHERE Email LIKE '%test%' OR Email LIKE '%@example.com%'");

            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM Team WHERE Name LIKE '%Test%'");

            await Context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM Project WHERE Name LIKE '%Test%'");
        }
        catch (Exception ex)
        {
            // Log the exception but don't throw to avoid breaking test setup
            System.Diagnostics.Debug.WriteLine($"Database cleanup failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates a unique email address for testing
    /// </summary>
    protected static string GenerateUniqueEmail() => $"test.{Guid.NewGuid():N}@example.com";

    /// <summary>
    /// Generates a unique name for testing
    /// </summary>
    protected static string GenerateUniqueName(string prefix = "Test") => $"{prefix} {Guid.NewGuid():N}";

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clean up after the test
            try
            {
                CleanupDatabase().Wait();
            }
            catch
            {
                // Ignore cleanup errors during disposal
            }

            Scope?.Dispose();
        }
    }
}
