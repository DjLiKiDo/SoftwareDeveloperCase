using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Moq;
using SoftwareDeveloperCase.Api.HealthChecks;
using SoftwareDeveloperCase.Application.Models;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.HealthChecks;

/// <summary>
/// Tests for EmailServiceHealthCheck
/// </summary>
public class EmailServiceHealthCheckTests
{
    /// <summary>
    /// Tests that health check returns degraded when SMTP server is not configured
    /// </summary>
    [Fact]
    public async System.Threading.Tasks.Task CheckHealthAsync_ShouldReturnDegraded_WhenSmtpServerNotConfigured()
    {
        // Arrange
        var emailSettings = new EmailSettings
        {
            SmtpServer = null,
            FromAddress = "test@example.com"
        };
        var mockOptions = new Mock<IOptions<EmailSettings>>();
        mockOptions.Setup(x => x.Value).Returns(emailSettings);

        var healthCheck = new EmailServiceHealthCheck(mockOptions.Object);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        result.Status.Should().Be(HealthStatus.Degraded);
        result.Description.Should().Be("SMTP server is not configured");
        result.Data.Should().ContainKey("smtp_server");
        result.Data["smtp_server"].Should().Be("not configured");
    }

    /// <summary>
    /// Tests that health check returns degraded when from address is not configured
    /// </summary>
    [Fact]
    public async System.Threading.Tasks.Task CheckHealthAsync_ShouldReturnDegraded_WhenFromAddressNotConfigured()
    {
        // Arrange
        var emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            FromAddress = null
        };
        var mockOptions = new Mock<IOptions<EmailSettings>>();
        mockOptions.Setup(x => x.Value).Returns(emailSettings);

        var healthCheck = new EmailServiceHealthCheck(mockOptions.Object);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        result.Status.Should().Be(HealthStatus.Degraded);
        result.Description.Should().Be("From address is not configured");
        result.Data.Should().ContainKey("from_address");
        result.Data["from_address"].Should().Be("not configured");
    }

    /// <summary>
    /// Tests that health check returns healthy when email service is properly configured
    /// </summary>
    [Fact]
    public async System.Threading.Tasks.Task CheckHealthAsync_ShouldReturnHealthy_WhenEmailServiceProperlyConfigured()
    {
        // Arrange
        var emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            FromAddress = "test@example.com",
            EnableSsl = true,
            Username = "user",
            Password = "pass"
        };
        var mockOptions = new Mock<IOptions<EmailSettings>>();
        mockOptions.Setup(x => x.Value).Returns(emailSettings);

        var healthCheck = new EmailServiceHealthCheck(mockOptions.Object);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Email service is properly configured");
        result.Data.Should().ContainKey("smtp_server");
        result.Data.Should().ContainKey("smtp_port");
        result.Data.Should().ContainKey("from_address");
        result.Data.Should().ContainKey("enable_ssl");
        result.Data.Should().ContainKey("has_credentials");
        result.Data["smtp_server"].Should().Be("smtp.example.com");
        result.Data["smtp_port"].Should().Be(587);
        result.Data["from_address"].Should().Be("test@example.com");
        result.Data["enable_ssl"].Should().Be(true);
        result.Data["has_credentials"].Should().Be(true);
    }

    /// <summary>
    /// Tests that health check returns unhealthy when an exception occurs
    /// </summary>
    [Fact]
    public async System.Threading.Tasks.Task CheckHealthAsync_ShouldReturnUnhealthy_WhenExceptionOccurs()
    {
        // Arrange
        var mockOptions = new Mock<IOptions<EmailSettings>>();
        mockOptions.SetupGet(x => x.Value).Throws(new InvalidOperationException("Test exception"));

        var healthCheck = new EmailServiceHealthCheck(mockOptions.Object);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("Error checking email service health");
        result.Exception.Should().BeOfType<InvalidOperationException>();
        result.Exception!.Message.Should().Be("Test exception");
    }
}
