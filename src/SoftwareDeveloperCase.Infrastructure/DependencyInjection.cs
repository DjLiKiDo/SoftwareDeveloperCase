using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Core;
using SoftwareDeveloperCase.Application.Contracts.Persistence.Identity;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Application.Validation;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Repositories.Cached;
using SoftwareDeveloperCase.Infrastructure.ExternalServices;

namespace SoftwareDeveloperCase.Infrastructure;

/// <summary>
/// Provides extension methods for configuring infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register core repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        // Register repository base classes
        services.AddScoped<RoleRepository>();
        services.AddScoped<PermissionRepository>();

        // Register repositories with caching decorators
        services.AddScoped<IRoleRepository>(provider =>
        {
            var baseRepository = provider.GetRequiredService<RoleRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            var cacheKeyService = provider.GetRequiredService<ICacheKeyService>();
            return new CachedRoleRepository(baseRepository, cache, cacheKeyService);
        });

        services.AddScoped<IPermissionRepository>(provider =>
        {
            var baseRepository = provider.GetRequiredService<PermissionRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            var cacheKeyService = provider.GetRequiredService<ICacheKeyService>();
            return new CachedPermissionRepository(baseRepository, cache, cacheKeyService);
        });

        // Register Core domain repositories
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();

        // Configure database context
        services.AddDbContext<Persistence.SoftwareDeveloperCaseDbContext>((serviceProvider, options) =>
        {
            var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            if (databaseSettings.UseInMemoryDatabase)
            {
                options.UseInMemoryDatabase("SoftwareDeveloperCaseInMemoryDb");
            }
            else
            {
                var connectionString = databaseSettings.ConnectionString ??
                    Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                    configuration.GetConnectionString("ConnectionString");

                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(databaseSettings.CommandTimeoutSeconds);
                });
            }

            if (databaseSettings.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (databaseSettings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Register Unit of Work after all repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register database interceptors
        services.AddScoped<Persistence.EntitySaveChangesInterceptor>();

        // Register services
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddTransient<ICacheKeyService, CacheKeyService>();

        // Configure strongly-typed configuration with validation
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SECTION_NAME));
        // Register configuration validators
        services.AddSingleton<IValidateOptions<DatabaseSettings>, DatabaseSettingsValidator>();

        services.Configure<EmailSettings>(options =>
        {
            configuration.GetSection(EmailSettings.SECTION_NAME).Bind(options);

            // Override with environment variables if available
            options.Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? options.Username;
            options.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? options.Password;
            options.FromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? options.FromAddress;
        });
        // Register email settings validator
        services.AddSingleton<IValidateOptions<EmailSettings>, EmailSettingsValidator>();
        
        // Register EmailService if needed
        services.AddTransient<IEmailService, EmailService>();

        // Add caching services
        services.AddMemoryCache();

        return services;
    }
}
