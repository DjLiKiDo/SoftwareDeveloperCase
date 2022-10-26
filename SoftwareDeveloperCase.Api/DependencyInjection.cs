using SoftwareDeveloperCase.Application;
using SoftwareDeveloperCase.Infrastructure;

namespace SoftwareDeveloperCase.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services
                .AddApplicationServices()
                .AddInfrastructureServices(configuration);

            services.AddLogging(logging => { logging.AddConsole(); });

            return services;
        }
    }
}
