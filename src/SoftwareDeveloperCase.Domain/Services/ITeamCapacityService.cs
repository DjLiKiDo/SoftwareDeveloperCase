using SoftwareDeveloperCase.Domain.Entities.Team;

namespace SoftwareDeveloperCase.Domain.Services;

/// <summary>
/// Domain service for team capacity calculations and management.
/// </summary>
public interface ITeamCapacityService
{
    /// <summary>
    /// Calculates the total capacity of a team based on active members.
    /// </summary>
    /// <param name="team">The team to calculate capacity for.</param>
    /// <returns>The total capacity in hours per sprint.</returns>
    decimal CalculateTeamCapacity(Team team);

    /// <summary>
    /// Calculates the available capacity of a team considering current workload.
    /// </summary>
    /// <param name="team">The team to calculate available capacity for.</param>
    /// <param name="currentWorkload">Current workload in hours.</param>
    /// <returns>The available capacity in hours.</returns>
    decimal CalculateAvailableCapacity(Team team, decimal currentWorkload);

    /// <summary>
    /// Determines if a team can take on additional work.
    /// </summary>
    /// <param name="team">The team to check.</param>
    /// <param name="additionalWorkload">Additional workload in hours.</param>
    /// <returns>True if the team can handle the additional workload.</returns>
    bool CanTakeOnWork(Team team, decimal additionalWorkload);

    /// <summary>
    /// Gets the optimal team size for a given project scope.
    /// </summary>
    /// <param name="estimatedHours">Estimated project hours.</param>
    /// <param name="timelineInWeeks">Project timeline in weeks.</param>
    /// <returns>Recommended team size.</returns>
    int GetOptimalTeamSize(decimal estimatedHours, int timelineInWeeks);
}
