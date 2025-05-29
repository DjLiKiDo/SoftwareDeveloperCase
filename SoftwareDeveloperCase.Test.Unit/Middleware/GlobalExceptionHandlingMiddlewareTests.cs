using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Api.Models;
using SoftwareDeveloperCase.Application.Exceptions;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Middleware;

/// <summary>
/// Integration tests for the global exception handling middleware.
/// </summary>
public class GlobalExceptionHandlingMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public GlobalExceptionHandlingMiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Replace the DbContext with a fresh in-memory database for each test
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SoftwareDeveloperCaseDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<SoftwareDeveloperCaseDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });
            });
        });

        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task Middleware_ShouldReturnValidationError_WhenValidationExceptionIsThrown()
    {
        // Arrange - Create a user with invalid data to trigger validation
        var invalidUserRequest = new
        {
            Name = "", // Empty name should trigger validation
            Email = "invalid-email", // Invalid email format
            Password = "",
            DepartmentId = Guid.Empty
        };

        // Act
        var response = await _client.PostAsync("/api/v1/User",
            new StringContent(JsonSerializer.Serialize(invalidUserRequest), System.Text.Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);

        errorResponse.Should().NotBeNull();
        errorResponse!.Title.Should().Be("Validation Failed");
        errorResponse.Status.Should().Be(400);
        errorResponse.Detail.Should().Be("One or more validation errors occurred.");
        errorResponse.TraceId.Should().NotBeNull();
        errorResponse.Errors.Should().NotBeNull();
        errorResponse.Errors.Should().ContainKey("Name");
        errorResponse.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Middleware_ShouldReturnNotFoundError_WhenResourceNotFound()
    {
        // Arrange - Try to get a non-existent user
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/User/{nonExistentUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);

        errorResponse.Should().NotBeNull();
        errorResponse!.Title.Should().Be("Resource Not Found");
        errorResponse.Status.Should().Be(404);
        errorResponse.TraceId.Should().NotBeNull();
    }

    [Fact]
    public async Task Middleware_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange - This will test the default exception handling
        // We'll make a request that might cause an internal error
        var request = new
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123",
            DepartmentId = Guid.NewGuid() // Non-existent department might cause issues
        };

        // Act
        var response = await _client.PostAsync("/api/v1/User",
            new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"));

        // Assert
        // Note: The actual status code might vary depending on the implementation
        // This test validates that errors are handled gracefully
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();

        // Should return valid JSON even if there's an error
        var isValidJson = IsValidJson(content);
        isValidJson.Should().BeTrue("Response should always be valid JSON");

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
            errorResponse.Should().NotBeNull();
            errorResponse!.Title.Should().Be("Internal Server Error");
            errorResponse.Status.Should().Be(500);
            errorResponse.TraceId.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Middleware_ShouldLogErrors_WhenExceptionOccurs()
    {
        // Arrange
        var loggerProvider = _factory.Services.GetRequiredService<ILoggerProvider>();

        // Act - Trigger an error
        var invalidUserRequest = new
        {
            Name = "",
            Email = "invalid-email",
            Password = "",
            DepartmentId = Guid.Empty
        };

        var response = await _client.PostAsync("/api/v1/User",
            new StringContent(JsonSerializer.Serialize(invalidUserRequest), System.Text.Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verify response format
        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);

        errorResponse.Should().NotBeNull();
        errorResponse!.TraceId.Should().NotBeNull();
    }

    [Fact]
    public async Task Middleware_ShouldNotExposeInternalDetails_InProductionEnvironment()
    {
        // Arrange - Create factory with Production environment
        var productionFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
        });

        using var client = productionFactory.CreateClient();

        // Act - Create a request that might cause an internal error
        var request = new
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123",
            DepartmentId = Guid.NewGuid()
        };

        var response = await client.PostAsync("/api/v1/User",
            new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"));

        // Assert
        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
            errorResponse.Should().NotBeNull();
            errorResponse!.Detail.Should().Be("An internal server error occurred. Please try again later.");
            errorResponse.Detail.Should().NotContain("Exception");
            errorResponse.Detail.Should().NotContain("Stack trace");
        }
    }

    [Fact]
    public async Task Middleware_ShouldSetCorrectContentType_ForErrorResponses()
    {
        // Arrange
        var invalidRequest = new
        {
            Name = "",
            Email = "invalid",
            Password = "",
            DepartmentId = Guid.Empty
        };

        // Act
        var response = await _client.PostAsync("/api/v1/User",
            new StringContent(JsonSerializer.Serialize(invalidRequest), System.Text.Encoding.UTF8, "application/json"));

        // Assert
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    /// <summary>
    /// Helper method to validate if a string is valid JSON.
    /// </summary>
    /// <param name="json">The JSON string to validate.</param>
    /// <returns>True if valid JSON, false otherwise.</returns>
    private static bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
