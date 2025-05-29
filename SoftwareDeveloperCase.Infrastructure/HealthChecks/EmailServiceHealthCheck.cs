using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Infrastructure.HealthChecks;

/// <summary>
/// Health check for email service availability
/// </summary>
public class EmailServiceHealthCheck : IHealthCheck
{
    private readonly EmailSettings _emailSettings;

    /// <summary>
    /// Initializes a new instance of the EmailServiceHealthCheck class
    /// </summary>
    /// <param name="emailSettings">The email configuration settings</param>
    public EmailServiceHealthCheck(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    /// <summary>
    /// Performs the health check for email service
    /// </summary>
    /// <param name="context">The health check context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The health check result</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if essential email settings are configured
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("SMTP server is not configured"));
            }

            if (string.IsNullOrEmpty(_emailSettings.FromAddress))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("From address is not configured"));
            }

            if (_emailSettings.SmtpPort <= 0)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("SMTP port is not configured properly"));
            }

            // If all basic settings are present, consider the service healthy
            // Note: We don't actually try to connect to SMTP server in health check to avoid performance issues
            var data = new Dictionary<string, object>
            {
                ["SmtpServer"] = _emailSettings.SmtpServer,
                ["SmtpPort"] = _emailSettings.SmtpPort,
                ["EnableSsl"] = _emailSettings.EnableSsl,
                ["FromAddress"] = _emailSettings.FromAddress
            };

            return Task.FromResult(HealthCheckResult.Healthy("Email service is properly configured", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Email service health check failed", ex));
        }
    }
}