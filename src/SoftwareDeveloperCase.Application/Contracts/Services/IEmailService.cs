using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Contracts.Services;

/// <summary>
/// Service interface for sending email notifications
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email message
    /// </summary>
    /// <param name="email">The email message to send</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the email was sent successfully, false otherwise</returns>
    Task<bool> SendEmail(Email email, CancellationToken cancellationToken = default);
}
