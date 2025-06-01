using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoftwareDeveloperCase.Api.Configuration;

/// <summary>
/// Operation filter to add standard response types to all endpoints
/// </summary>
public class ResponseTypeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common error responses
        operation.Responses.TryAdd("400", new OpenApiResponse
        {
            Description = "Bad Request - Invalid input data",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "ProblemDetails"
                        }
                    }
                }
            }
        });

        operation.Responses.TryAdd("401", new OpenApiResponse
        {
            Description = "Unauthorized - Authentication required"
        });

        operation.Responses.TryAdd("403", new OpenApiResponse
        {
            Description = "Forbidden - Insufficient permissions"
        });

        operation.Responses.TryAdd("500", new OpenApiResponse
        {
            Description = "Internal Server Error - Unexpected error occurred",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "ProblemDetails"
                        }
                    }
                }
            }
        });
    }
}
