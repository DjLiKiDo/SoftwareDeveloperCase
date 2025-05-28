using SoftwareDeveloperCase.Application.Contracts.Services;

namespace SoftwareDeveloperCase.Infrastructure.Services
{
    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
