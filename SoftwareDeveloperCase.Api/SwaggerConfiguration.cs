namespace SoftwareDeveloperCase.Api;

/// <summary>
/// Provides extension methods for configuring Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configures Swagger UI and OpenAPI documentation for development environment.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
