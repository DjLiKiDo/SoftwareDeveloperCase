using SoftwareDeveloperCase.Application;
using SoftwareDeveloperCase.Infrastructure;

namespace SoftwareDeveloperCase.Api
{
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

            services
                .AddApplicationServices()
                .AddInfrastructureServices(configuration);

            return services;
        }
    }
}
