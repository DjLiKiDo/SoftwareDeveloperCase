using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoftwareDeveloperCase.Api.Configuration;

/// <summary>
/// Schema filter to display enum values as strings in Swagger UI
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the enum schema filter to convert enum values to strings in Swagger
    /// </summary>
    /// <param name="schema">The OpenAPI schema</param>
    /// <param name="context">The schema filter context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumValue.ToString()));
            }
        }
    }
}
