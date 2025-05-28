using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using SoftwareDeveloperCase.Infrastructure.Repositories;
using SoftwareDeveloperCase.Infrastructure.Services;

namespace SoftwareDeveloperCase.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"))
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<EntitySaveChangesInterceptor>();

            services.AddSingleton<IDateTimeService, DateTimeService>();

            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SECTION_NAME));
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
