using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

namespace SoftwareDeveloperCase.Test.Integration.Common;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing database context registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SoftwareDeveloperCaseDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add in-memory database for testing
            services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
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
                // Ensure the database is created
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
}
