using SoftwareDeveloperCase.Application;
using SoftwareDeveloperCase.Infrastructure;
using SoftwareDeveloperCase.Api.HealthChecks;
using SoftwareDeveloperCase.Infrastructure.Persistence;

namespace SoftwareDeveloperCase.Api;

/// <summary>
/// Provides extension methods for configuring services in the API layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all required services for the API layer including controllers, Swagger, and dependencies from other layers.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<SoftwareDeveloperCaseDbContext>("database")
            .AddCheck<EmailServiceHealthCheck>("email_service");

        services
            .AddApplicationServices()
            .AddInfrastructureServices(configuration);

        return services;
    }
}
