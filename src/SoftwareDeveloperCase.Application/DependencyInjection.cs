using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Behaviours;
using System.Reflection;

namespace SoftwareDeveloperCase.Application;

/// <summary>
/// Contains extension methods for configuring application services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SanitizationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}
