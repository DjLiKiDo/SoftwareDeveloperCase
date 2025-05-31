using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoftwareDeveloperCase.Api.Configuration;

/// <summary>
/// Schema filter to display enum values as strings in Swagger UI
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
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
