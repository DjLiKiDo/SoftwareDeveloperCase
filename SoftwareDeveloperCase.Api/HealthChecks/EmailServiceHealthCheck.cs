using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Api.HealthChecks;

/// <summary>
/// Health check for email service configuration and availability
/// </summary>
public class EmailServiceHealthCheck : IHealthCheck
{
    private readonly IOptions<EmailSettings> _emailSettingsOptions;

    /// <summary>
    /// Initializes a new instance of the EmailServiceHealthCheck class
    /// </summary>
    /// <param name="emailSettings">The email configuration settings</param>
    public EmailServiceHealthCheck(IOptions<EmailSettings> emailSettings)
    {
        _emailSettingsOptions = emailSettings;
    }

    /// <summary>
    /// Checks the health of the email service configuration
    /// </summary>
    /// <param name="context">The health check context</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The health check result</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailSettings = _emailSettingsOptions.Value;
            var data = new Dictionary<string, object>();

            // Check if SMTP server is configured
            if (string.IsNullOrEmpty(emailSettings.SmtpServer))
            {
                data["smtp_server"] = "not configured";
                return Task.FromResult(HealthCheckResult.Degraded("SMTP server is not configured", data: data));
            }

            // Check if from address is configured
            if (string.IsNullOrEmpty(emailSettings.FromAddress))
            {
                data["from_address"] = "not configured";
                return Task.FromResult(HealthCheckResult.Degraded("From address is not configured", data: data));
            }

            // Email service is properly configured
            data["smtp_server"] = emailSettings.SmtpServer;
            data["smtp_port"] = emailSettings.SmtpPort;
            data["from_address"] = emailSettings.FromAddress;
            data["enable_ssl"] = emailSettings.EnableSsl;
            data["has_credentials"] = !string.IsNullOrEmpty(emailSettings.Username);

            return Task.FromResult(HealthCheckResult.Healthy("Email service is properly configured", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Error checking email service health", ex));
        }
    }
}