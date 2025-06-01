using Microsoft.OpenApi.Models;
using System.Reflection;
using SoftwareDeveloperCase.Api.Configuration;

namespace SoftwareDeveloperCase.Api;

/// <summary>
/// Provides extension methods for configuring Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adds Swagger/OpenAPI services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // API Information
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "SoftwareDeveloperCase API",
                Description = "A comprehensive project management API built with Clean Architecture",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "dev@softwaredevelopercase.com",
                    Url = new Uri("https://github.com/your-org/SoftwareDeveloperCase")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Include Application layer XML comments if available
            var applicationXmlFile = "SoftwareDeveloperCase.Application.xml";
            var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
            if (File.Exists(applicationXmlPath))
            {
                c.IncludeXmlComments(applicationXmlPath);
            }

            // Configure schema for enums to show string values
            c.SchemaFilter<EnumSchemaFilter>();

            // Add custom operation filters
            c.OperationFilter<CorrelationIdOperationFilter>();
            c.OperationFilter<ResponseTypeOperationFilter>();

            // Group endpoints by tags/domains
            c.TagActionsBy(api =>
            {
                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return controllerName switch
                {
                    "Users" => new[] { "Identity Management" },
                    "Roles" => new[] { "Identity Management" },
                    "Teams" => new[] { "Team Management" },
                    "Projects" => new[] { "Project Management" },
                    "Tasks" => new[] { "Task Management" },
                    _ => new[] { controllerName ?? "General" }
                };
            });

            // Enable annotations for better documentation
            c.EnableAnnotations();

            // Configure polymorphism support
            c.UseAllOfToExtendReferenceSchemas();
            c.UseAllOfForInheritance();
            c.UseOneOfForPolymorphism();
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI and OpenAPI documentation for development environment.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureSwagger(this WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                swagger.Servers = new List<OpenApiServer>
                {
                    new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                };
            });
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SoftwareDeveloperCase API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "SoftwareDeveloperCase API Documentation";

            // UI Customization
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayRequestDuration();
            c.EnableValidator();
            c.ShowExtensions();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.MaxDisplayedTags(50);

            // Custom CSS for better appearance
            c.InjectStylesheet("/swagger-ui/custom.css");

            // OAuth2 configuration if needed
            c.OAuthClientId("swagger-ui");
            c.OAuthAppName("SoftwareDeveloperCase API");
            c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });

        return app;
    }
}
