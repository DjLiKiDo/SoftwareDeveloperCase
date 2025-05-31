using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.ValueObjects;

/// <summary>
/// Represents a project timeline with start and end dates.
/// </summary>
public class ProjectTimeline : ValueObject
{
    /// <summary>
    /// Gets the planned start date.
    /// </summary>
    public DateTime PlannedStartDate { get; }

    /// <summary>
    /// Gets the planned end date.
    /// </summary>
    public DateTime PlannedEndDate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectTimeline"/> class.
    /// </summary>
    /// <param name="plannedStartDate">The planned start date.</param>
    /// <param name="plannedEndDate">The planned end date.</param>
    public ProjectTimeline(DateTime plannedStartDate, DateTime plannedEndDate)
    {
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The timeline components for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlannedStartDate;
        yield return PlannedEndDate;
    }

    /// <summary>
    /// Returns the string representation of the project timeline.
    /// </summary>
    /// <returns>The timeline formatted with planned dates.</returns>
    public override string ToString() =>
        $"Planned: {PlannedStartDate:yyyy-MM-dd} to {PlannedEndDate:yyyy-MM-dd}";
}
