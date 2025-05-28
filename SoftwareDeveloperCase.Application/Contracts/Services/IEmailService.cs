using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Contracts.Services;

public interface IEmailService
{
    Task<bool> SendEmail(Email email, CancellationToken cancellationToken = default);
}
