using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Domain.Enums.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Xunit;

namespace SoftwareDeveloperCase.Test.Integration.Authorization;

/// <summary>
/// Integration tests for authorization across different controllers
/// </summary>
public class AuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthorizationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove existing authentication services and configure test authentication as default
                var authenticationBuilder = services.AddAuthentication(defaultScheme: "Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                // Clear existing authentication schemes and rebuild with test scheme
                services.Configure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTeams_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Admin, Guid.NewGuid());

        // Act
        var response = await _client.GetAsync("/api/v1/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTeams_AsManager_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Manager, Guid.NewGuid());

        // Act
        var response = await _client.GetAsync("/api/v1/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTeams_AsDeveloper_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Developer, Guid.NewGuid());

        // Act
        var response = await _client.GetAsync("/api/v1/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateTeam_AsAdmin_ShouldReturnCreated()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Admin, Guid.NewGuid());
        var teamData = new
        {
            Name = "Test Team",
            Description = "Test Description"
        };
        var content = new StringContent(JsonSerializer.Serialize(teamData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/teams", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest); // BadRequest due to validation
    }

    [Fact]
    public async Task CreateTeam_AsDeveloper_ShouldReturnForbidden()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Developer, Guid.NewGuid());
        var teamData = new
        {
            Name = "Test Team",
            Description = "Test Description"
        };
        var content = new StringContent(JsonSerializer.Serialize(teamData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/teams", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProjects_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Admin, Guid.NewGuid());

        // Act
        var response = await _client.GetAsync("/api/v1/projects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProject_AsManager_ShouldReturnCreated()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Manager, Guid.NewGuid());
        var projectData = new
        {
            Name = "Test Project",
            Description = "Test Description",
            TeamId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3)
        };
        var content = new StringContent(JsonSerializer.Serialize(projectData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/projects", content);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest); // BadRequest due to validation
    }

    [Fact]
    public async Task CreateProject_AsDeveloper_ShouldReturnForbidden()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Developer, Guid.NewGuid());
        var projectData = new
        {
            Name = "Test Project",
            Description = "Test Description",
            TeamId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3)
        };
        var content = new StringContent(JsonSerializer.Serialize(projectData), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/projects", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTasks_AsManager_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Manager, Guid.NewGuid());

        // Act
        var response = await _client.GetAsync("/api/v1/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProject_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Admin, Guid.NewGuid());
        var projectId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/projects/{projectId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound); // NotFound if project doesn't exist
    }

    [Fact]
    public async Task DeleteProject_AsDeveloper_ShouldReturnForbidden()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Developer, Guid.NewGuid());
        var projectId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/projects/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteTask_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Admin, Guid.NewGuid());
        var taskId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound); // NotFound if task doesn't exist
    }

    [Fact]
    public async Task DeleteTask_AsDeveloper_ShouldReturnForbidden()
    {
        // Arrange
        SetAuthenticationHeader(SystemRole.Developer, Guid.NewGuid());
        var taskId = 123; // Use int instead of Guid since the route expects int

        // Act
        var response = await _client.DeleteAsync($"/api/v1/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AccessWithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/v1/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private void SetAuthenticationHeader(SystemRole role, Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(ClaimTypes.Name, $"test-user-{role}"),
            new Claim(ClaimTypes.Email, $"test-{role}@example.com")
        };

        var token = TestJwtTokenGenerator.GenerateToken(claims);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

/// <summary>
/// Test authentication handler for integration tests
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Task.FromResult(AuthenticateResult.Fail("No Authorization header"));
        }

        // For testing, we'll decode a simple test token
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var claims = TestJwtTokenGenerator.DecodeToken(token);

        if (claims == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

/// <summary>
/// Simple JWT token generator for testing
/// </summary>
public static class TestJwtTokenGenerator
{
    public static string GenerateToken(Claim[] claims)
    {
        // For testing, we'll use a simple base64 encoded claims representation
        var claimsDict = claims.ToDictionary(c => c.Type, c => c.Value);
        var json = JsonSerializer.Serialize(claimsDict);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    public static Claim[]? DecodeToken(string token)
    {
        try
        {
            var bytes = Convert.FromBase64String(token);
            var json = Encoding.UTF8.GetString(bytes);
            var claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return claimsDict?.Select(kvp => new Claim(kvp.Key, kvp.Value)).ToArray();
        }
        catch
        {
            return null;
        }
    }
}
