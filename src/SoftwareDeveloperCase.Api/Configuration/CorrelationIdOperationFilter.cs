using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoftwareDeveloperCase.Api.Configuration;

/// <summary>
/// Operation filter to add correlation ID parameter to all endpoints
/// </summary>
public class CorrelationIdOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the correlation ID operation filter to the Swagger specification
    /// </summary>
    /// <param name="operation">The OpenAPI operation</param>
    /// <param name="context">The operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Correlation-ID",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Correlation ID for request tracking",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid"
            }
        });
    }
}
