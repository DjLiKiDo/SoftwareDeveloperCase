using SoftwareDeveloperCase.Domain.Common;

namespace SoftwareDeveloperCase.Domain.ValueObjects;

/// <summary>
/// Represents a time range value object.
/// </summary>
public class TimeRange : ValueObject
{
    /// <summary>
    /// Gets the start date and time.
    /// </summary>
    public DateTime StartDate { get; }

    /// <summary>
    /// Gets the end date and time.
    /// </summary>
    public DateTime EndDate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> class.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <exception cref="ArgumentException">Thrown when start date is after end date.</exception>
    public TimeRange(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.");

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Gets the duration of the time range.
    /// </summary>
    public TimeSpan Duration => EndDate - StartDate;

    /// <summary>
    /// Gets the total days in the time range.
    /// </summary>
    public int TotalDays => (int)Duration.TotalDays;

    /// <summary>
    /// Checks if a given date falls within this time range.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is within the range, false otherwise.</returns>
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;

    /// <summary>
    /// Checks if this time range overlaps with another time range.
    /// </summary>
    /// <param name="other">The other time range.</param>
    /// <returns>True if the ranges overlap, false otherwise.</returns>
    public bool OverlapsWith(TimeRange other) => StartDate < other.EndDate && EndDate > other.StartDate;

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The start and end dates for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    /// <summary>
    /// Returns the string representation of the time range.
    /// </summary>
    /// <returns>The time range formatted as "StartDate to EndDate".</returns>
    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
