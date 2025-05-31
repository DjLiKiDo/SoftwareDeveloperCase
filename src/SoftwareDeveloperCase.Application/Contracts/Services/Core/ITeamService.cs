using SoftwareDeveloperCase.Domain.Entities.Team;

namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// Service interface for team management operations
/// </summary>
public interface ITeamService
{
    /// <summary>
    /// Validates team business rules before creation or update
    /// </summary>
    /// <param name="team">The team to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid, false otherwise</returns>
    Task<bool> ValidateTeamAsync(Team team, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user can be added to team
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user can be added, false otherwise</returns>
    Task<bool> CanAddUserToTeamAsync(int teamId, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates team workload metrics
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team workload information</returns>
    Task<TeamWorkloadDto> CalculateTeamWorkloadAsync(int teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets team performance metrics
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="startDate">Start date for metrics</param>
    /// <param name="endDate">End date for metrics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team performance metrics</returns>
    Task<TeamPerformanceDto> GetTeamPerformanceAsync(int teamId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
