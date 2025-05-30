using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Domain.Common;
using SoftwareDeveloperCase.Infrastructure.Persistence.Extensions;

namespace SoftwareDeveloperCase.Infrastructure.Persistence;

/// <summary>
/// Interceptor that automatically updates audit fields on entities during save operations.
/// </summary>
public class EntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeService _dateTimeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySaveChangesInterceptor"/> class.
    /// </summary>
    /// <param name="dateTimeService">The date time service for setting audit timestamps.</param>
    public EntitySaveChangesInterceptor(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }

    /// <summary>
    /// Intercepts synchronous save changes to update audit fields before saving.
    /// </summary>
    /// <param name="eventData">The event data containing context information.</param>
    /// <param name="result">The current interception result.</param>
    /// <returns>The interception result.</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts asynchronous save changes to update audit fields before saving.
    /// </summary>
    /// <param name="eventData">The event data containing context information.</param>
    /// <param name="result">The current interception result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous interception result.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Updates audit fields on entities that are being added or modified.
    /// </summary>
    /// <param name="context">The database context containing the entities to update.</param>
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
