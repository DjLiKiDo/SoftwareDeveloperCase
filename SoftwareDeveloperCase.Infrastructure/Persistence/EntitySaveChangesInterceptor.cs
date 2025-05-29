using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IDateTimeService _dateTimeService;

        public EntitySaveChangesInterceptor(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = "ApiUser";
                    entry.Entity.CreatedOn = _dateTimeService.Now;
                }

                if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.LastModifiedBy = "ApiUser";
                    entry.Entity.LastModifiedOn = _dateTimeService.Now;
                }
            }
        }
    }
}
