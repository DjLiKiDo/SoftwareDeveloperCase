using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using Testcontainers.MsSql;
using Xunit;

namespace SoftwareDeveloperCase.Test.Integration.Common;

/// <summary>
/// Custom WebApplicationFactory for integration tests using real SQL Server in container
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;

    public CustomWebApplicationFactory()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("TestPassword123!")
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing database context registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SoftwareDeveloperCaseDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add SQL Server database for testing with Testcontainers
            services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
            {
                options.UseSqlServer(_msSqlContainer.GetConnectionString());
                options.EnableSensitiveDataLogging();
            });

            // Suppress logging during tests
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to get required services
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<SoftwareDeveloperCaseDbContext>();

            try
            {
                // Ensure the database is created and migrations applied
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();
                logger.LogError(ex, "An error occurred creating the test database");
                throw;
            }
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
