using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.DTOs.Auth;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer;
using SoftwareDeveloperCase.Test.Integration.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SoftwareDeveloperCase.Test.Integration.Controllers;

/// <summary>
/// Integration tests for AuthController
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnAuthenticationResponse()
    {
        // Arrange
        await SeedTestDataAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthenticationResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Email.Should().Be("test@example.com");
        authResponse.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        await SeedTestDataAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        await SeedTestDataAsync();
        var authResponse = await LoginAsync();

        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = authResponse.RefreshToken
        };

        var json = JsonSerializer.Serialize(refreshRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/auth/refresh", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var newAuthResponse = JsonSerializer.Deserialize<AuthenticationResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        newAuthResponse.Should().NotBeNull();
        newAuthResponse!.AccessToken.Should().NotBeNullOrEmpty();
        newAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
        newAuthResponse.AccessToken.Should().NotBe(authResponse.AccessToken);
        newAuthResponse.RefreshToken.Should().NotBe(authResponse.RefreshToken);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ShouldReturnUserInfo()
    {
        // Arrange
        await SeedTestDataAsync();
        var authResponse = await LoginAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<UserInfo>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        userInfo.Should().NotBeNull();
        userInfo!.Email.Should().Be("test@example.com");
        userInfo.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnNoContent()
    {
        // Arrange
        await SeedTestDataAsync();
        var authResponse = await LoginAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        var logoutRequest = new RefreshTokenRequest
        {
            RefreshToken = authResponse.RefreshToken
        };

        var json = JsonSerializer.Serialize(logoutRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/auth/logout", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<AuthenticationResponse> LoginAsync()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/auth/login", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthenticationResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SoftwareDeveloperCaseDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        // Clear existing data
        context.Users?.RemoveRange(context.Users);
        context.Roles?.RemoveRange(context.Roles);
        context.UserRoles?.RemoveRange(context.UserRoles);
        await context.SaveChangesAsync();

        // Create test role
        var developerRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Developer",
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };
        context.Roles?.Add(developerRole);

        // Create test user
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = new SoftwareDeveloperCase.Domain.ValueObjects.Email("test@example.com"),
            Password = passwordService.HashPassword("TestPassword123!"),
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };
        context.Users?.Add(testUser);

        // Create user role relationship
        var userRole = new UserRole
        {
            UserId = testUser.Id,
            RoleId = developerRole.Id,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };
        context.UserRoles?.Add(userRole);

        await context.SaveChangesAsync();
    }
}
