using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using SoftwareDeveloperCase.Infrastructure.Repositories;
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
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
            options.UseInMemoryDatabase("SoftwareDeveloperCaseInMemoryDb")
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<EntitySaveChangesInterceptor>();

        services.AddSingleton<IDateTimeService, DateTimeService>();

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SECTION_NAME));
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}
