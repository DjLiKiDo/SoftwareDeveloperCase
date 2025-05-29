using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using SoftwareDeveloperCase.Infrastructure.Repositories;
using SoftwareDeveloperCase.Infrastructure.Repositories.Cached;
using SoftwareDeveloperCase.Infrastructure.Services;

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
        services.AddScoped<UserRoleRepository>();
        services.AddScoped<RolePermissionRepository>();

        // Register repositories with caching decorators
        services.AddScoped<RoleRepository>();
        services.AddScoped<IRoleRepository>(provider =>
        {
            var baseRepository = provider.GetRequiredService<RoleRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new CachedRoleRepository(baseRepository, cache);
        });

        services.AddScoped<PermissionRepository>();
        services.AddScoped<IPermissionRepository>(provider =>
        {
            var baseRepository = provider.GetRequiredService<PermissionRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new CachedPermissionRepository(baseRepository, cache);
        });

        services.AddScoped<DepartmentRepository>();
        services.AddScoped<IDepartmentRepository>(provider =>
        {
            var baseRepository = provider.GetRequiredService<DepartmentRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new CachedDepartmentRepository(baseRepository, cache);
        });

        services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
            options.UseInMemoryDatabase("SoftwareDeveloperCaseInMemoryDb")
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<EntitySaveChangesInterceptor>();

        services.AddSingleton<IDateTimeService, DateTimeService>();

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SECTION_NAME));
        services.AddTransient<IEmailService, EmailService>();

        // Add caching services
        services.AddMemoryCache();

        return services;
    }
}
