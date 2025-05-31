using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using System.Net;
using System.Net.Mail;
using ApplicationEmail = SoftwareDeveloperCase.Application.Models.Email;

namespace SoftwareDeveloperCase.Infrastructure.ExternalServices;

/// <summary>
/// Service implementation for sending email notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmailService class
    /// </summary>
    /// <param name="emailSettings">The email configuration settings</param>
    /// <param name="logger">The logger instance</param>
    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email message using SMTP
    /// </summary>
    /// <param name="email">The email message to send</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the email was sent successfully, false otherwise</returns>
    public async Task<bool> SendEmail(ApplicationEmail email, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate email settings
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
            {
                _logger.LogError("SMTP server is not configured");
                return false;
            }

            if (string.IsNullOrEmpty(_emailSettings.FromAddress))
            {
                _logger.LogError("From address is not configured");
                return false;
            }

            // Validate email parameters
            if (string.IsNullOrEmpty(email.To))
            {
                _logger.LogError("To address is required");
                return false;
            }

            if (string.IsNullOrEmpty(email.Subject) && string.IsNullOrEmpty(email.Body))
            {
                _logger.LogError("Email must have either subject or body");
                return false;
            }

            _logger.LogInformation("Sending email to {To} with subject '{Subject}'", email.To, email.Subject);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort);

            // Configure SMTP client
            smtpClient.EnableSsl = _emailSettings.EnableSsl;

            if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            }

            // Create mail message
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName);
            mailMessage.To.Add(email.To);
            mailMessage.Subject = email.Subject ?? string.Empty;
            mailMessage.Body = email.Body ?? string.Empty;
            mailMessage.IsBodyHtml = !string.IsNullOrEmpty(email.Body) && email.Body.Contains("<");

            // Send email
            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", email.To);
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "SMTP error occurred while sending email to {To}: {Message}", email.To, ex.Message);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Configuration error while sending email to {To}: {Message}", email.To, ex.Message);
            return false;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Email sending was cancelled for {To}", email.To);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while sending email to {To}: {Message}", email.To, ex.Message);
            return false;
        }
    }
}
