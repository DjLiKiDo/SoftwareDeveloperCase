using SoftwareDeveloperCase.Application.Contracts.Services;

namespace SoftwareDeveloperCase.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for providing date and time operations
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <summary>
        /// Gets the current date and time in UTC
        /// </summary>
        public DateTime Now => DateTime.UtcNow;
    }
}
