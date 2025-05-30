using FluentAssertions;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Extensions;
using SoftwareDeveloperCase.Application.Models;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Validation;

/// <summary>
/// Tests for configuration validation
/// </summary>
public class ConfigurationValidationTests
{
    /// <summary>
    /// Tests that valid email settings pass validation
    /// </summary>
    [Fact]
    public void ValidateEmailSettings_WithValidSettings_ShouldReturnSuccess()
    {
        // Arrange
        var emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            EnableSsl = true,
            FromAddress = "test@example.com",
            FromName = "Test App",
            Username = "testuser",
            Password = "testpass"
        };

        // Act
        var result = emailSettings.ValidateEmailSettings();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
    }

    /// <summary>
    /// Tests that invalid email settings fail validation
    /// </summary>
    [Fact]
    public void ValidateEmailSettings_WithInvalidSettings_ShouldReturnFailure()
    {
        // Arrange
        var emailSettings = new EmailSettings
        {
            SmtpServer = "", // Invalid empty server
            SmtpPort = 70000, // Invalid port
            FromAddress = "invalid-email", // Invalid email format
            FromName = "" // Invalid empty name
        };

        // Act
        var result = emailSettings.ValidateEmailSettings();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that valid database settings pass validation
    /// </summary>
    [Fact]
    public void ValidateDatabaseSettings_WithValidInMemorySettings_ShouldReturnSuccess()
    {
        // Arrange
        var databaseSettings = new DatabaseSettings
        {
            UseInMemoryDatabase = true,
            DatabaseProvider = "InMemory",
            CommandTimeoutSeconds = 30
        };

        // Act
        var result = databaseSettings.ValidateDatabaseSettings();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
    }

    /// <summary>
    /// Tests that database settings without connection string when not using in-memory fail validation
    /// </summary>
    [Fact]
    public void ValidateDatabaseSettings_WithoutConnectionStringWhenNotInMemory_ShouldReturnFailure()
    {
        // Arrange
        var databaseSettings = new DatabaseSettings
        {
            UseInMemoryDatabase = false,
            DatabaseProvider = "SqlServer",
            ConnectionString = null // Missing connection string
        };

        // Act
        var result = databaseSettings.ValidateDatabaseSettings();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().ContainMatch("*ConnectionString is required*");
    }

    /// <summary>
    /// Tests that database settings with invalid provider fail validation
    /// </summary>
    [Fact]
    public void ValidateDatabaseSettings_WithInvalidProvider_ShouldReturnFailure()
    {
        // Arrange
        var databaseSettings = new DatabaseSettings
        {
            DatabaseProvider = "InvalidProvider", // Invalid provider
            CommandTimeoutSeconds = 30
        };

        // Act
        var result = databaseSettings.ValidateDatabaseSettings();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Failures.Should().ContainMatch("*DatabaseProvider must be one of*");
    }
}
