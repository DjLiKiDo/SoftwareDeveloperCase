namespace SoftwareDeveloperCase.Domain.Enums.Core;

/// <summary>
/// Represents the status of a project.
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Project is in planning phase.
    /// </summary>
    Planning = 0,

    /// <summary>
    /// Project is actively being worked on.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Project is temporarily on hold.
    /// </summary>
    OnHold = 2,

    /// <summary>
    /// Project has been completed.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Project has been cancelled.
    /// </summary>
    Cancelled = 4
}
