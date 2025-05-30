using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using FluentAssertions;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Api;

/// <summary>
/// Tests for health check endpoints
/// </summary>
public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// Initializes a new instance of the HealthCheckTests class
    /// </summary>
    /// <param name="factory">The web application factory</param>
    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Tests that the health check endpoint returns OK status
    /// </summary>
    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Tests that the health check endpoint returns health status content
    /// </summary>
    [Fact]
    public async Task HealthCheck_ShouldReturnDegradedStatus_WhenEmailServiceNotConfigured()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("Degraded");
    }

    /// <summary>
    /// Tests that the detailed health check endpoint returns JSON with health check details
    /// </summary>
    [Fact]
    public async Task HealthCheckDetailed_ShouldReturnJsonWithHealthDetails()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/detailed");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        content.Should().Contain("database");
        content.Should().Contain("email_service");
        content.Should().Contain("Degraded");
    }
}
