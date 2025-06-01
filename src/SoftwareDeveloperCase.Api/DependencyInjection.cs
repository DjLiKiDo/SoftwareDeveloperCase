using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SoftwareDeveloperCase.Application;
using SoftwareDeveloperCase.Application.Models;
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
        // Configure JWT authentication
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configure authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Admin"));
            options.AddPolicy("DeveloperOrManager", policy => policy.RequireRole("Developer", "Manager", "Admin"));
        });

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
