using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SoftwareDeveloperCase.Infrastructure.Persistence;
using SoftwareDeveloperCase.Application.Models;
using Microsoft.Extensions.Options;
using System.Net;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.HealthChecks;

/// <summary>
/// Integration tests for health check endpoints
/// </summary>
public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase($"HealthCheckTestDb_{Guid.NewGuid()}");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnUnhealthy_WhenEmailServiceIsNotConfigured()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Unhealthy");
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy_WhenEmailServiceIsProperlyConfigured()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
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
                    options.UseInMemoryDatabase($"HealthCheckTestDb_{Guid.NewGuid()}");
                });

                // Configure email settings for test
                services.Configure<EmailSettings>(options =>
                {
                    options.SmtpServer = "smtp.test.com";
                    options.SmtpPort = 587;
                    options.FromAddress = "test@test.com";
                    options.EnableSsl = true;
                    options.Username = "testuser";
                    options.Password = "testpass";
                });
            });
        });

        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task HealthCheckReady_ShouldReturnHealthy_WhenReadyTaggedServicesAreWorking()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task HealthCheckLive_ShouldReturnHealthy_WhenNoSpecificChecksAreRequired()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}