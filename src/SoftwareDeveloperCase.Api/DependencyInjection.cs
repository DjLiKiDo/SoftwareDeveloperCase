using SoftwareDeveloperCase.Application;
using SoftwareDeveloperCase.Infrastructure;
using SoftwareDeveloperCase.Api.HealthChecks;
using SoftwareDeveloperCase.Api.Filters;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;

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
        // Configure controllers with filters
        services.AddControllers(options =>
        {
            options.Filters.Add<ModelValidationActionFilter>();
            options.Filters.Add<PerformanceLoggingActionFilter>();
            options.Filters.Add<ResourceAccessAuthorizationFilter>();
        });

        // Configure API filters
        services.AddScoped<ModelValidationActionFilter>();
        services.AddScoped<PerformanceLoggingActionFilter>();
        services.AddScoped<ResourceAccessAuthorizationFilter>();

        // Configure Swagger with enhanced documentation
        services.AddSwaggerDocumentation();

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
