using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Infrastructure.Services;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Services;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly EmailSettings _emailSettings;
    private readonly Mock<IOptions<EmailSettings>> _mockOptions;

    public EmailServiceTests()
    {
        _mockLogger = new Mock<ILogger<EmailService>>();
        _emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.test.com",
            SmtpPort = 587,
            Username = "test@test.com",
            Password = "password",
            EnableSsl = true,
            FromAddress = "noreply@test.com",
            FromName = "Test System"
        };
        _mockOptions = new Mock<IOptions<EmailSettings>>();
        _mockOptions.Setup(x => x.Value).Returns(_emailSettings);
    }

    [Fact]
    public void EmailService_ShouldImplementIEmailService()
    {
        // Arrange & Act
        var emailServiceType = typeof(EmailService);
        var interfaceType = typeof(IEmailService);

        // Assert
        interfaceType.IsAssignableFrom(emailServiceType).Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithValidParameters()
    {
        // Arrange & Act
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);

        // Assert
        emailService.Should().NotBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithMissingSmtpServer_ShouldReturnFalseAndLogError()
    {
        // Arrange
        _emailSettings.SmtpServer = null;
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Test body"
        };

        // Act
        var result = await emailService.SendEmail(email);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SMTP server is not configured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithMissingFromAddress_ShouldReturnFalseAndLogError()
    {
        // Arrange
        _emailSettings.FromAddress = null;
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Test body"
        };

        // Act
        var result = await emailService.SendEmail(email);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("From address is not configured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithMissingToAddress_ShouldReturnFalseAndLogError()
    {
        // Arrange
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = null,
            Subject = "Test",
            Body = "Test body"
        };

        // Act
        var result = await emailService.SendEmail(email);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("To address is required")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithMissingSubjectAndBody_ShouldReturnFalseAndLogError()
    {
        // Arrange
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = "test@example.com",
            Subject = null,
            Body = null
        };

        // Act
        var result = await emailService.SendEmail(email);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email must have either subject or body")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithValidEmailButInvalidSmtp_ShouldReturnFalseAndLogSmtpError()
    {
        // Arrange
        // Use an invalid SMTP server to trigger SMTP exception
        _emailSettings.SmtpServer = "invalid.smtp.server";
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Test body"
        };

        // Act
        var result = await emailService.SendEmail(email);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SMTP error occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendEmail_WithCancellationToken_ShouldHandleCancellation()
    {
        // Arrange
        var emailService = new EmailService(_mockOptions.Object, _mockLogger.Object);
        var email = new Email
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Test body"
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // Cancel immediately

        // Act
        var result = await emailService.SendEmail(email, cancellationTokenSource.Token);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email sending was cancelled")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void EmailSettings_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var settings = new EmailSettings();

        // Assert
        settings.SmtpPort.Should().Be(587);
        settings.EnableSsl.Should().BeTrue();
        EmailSettings.SECTION_NAME.Should().Be("EmailSettings");
    }

    [Fact]
    public void IEmailService_ShouldHaveSendEmailMethod()
    {
        // Arrange
        var emailServiceType = typeof(IEmailService);

        // Act
        var sendEmailMethod = emailServiceType.GetMethod("SendEmail");

        // Assert
        sendEmailMethod.Should().NotBeNull();
        sendEmailMethod!.ReturnType.Should().Be(typeof(Task<bool>));
        sendEmailMethod.GetParameters().Should().HaveCount(2);
        sendEmailMethod.GetParameters()[0].ParameterType.Should().Be(typeof(Email));
        sendEmailMethod.GetParameters()[1].ParameterType.Should().Be(typeof(CancellationToken));
    }
}
