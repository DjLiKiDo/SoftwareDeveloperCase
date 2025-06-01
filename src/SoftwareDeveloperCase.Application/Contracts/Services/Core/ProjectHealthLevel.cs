namespace SoftwareDeveloperCase.Application.Contracts.Services.Core;

/// <summary>
/// Enumeration for project health levels
/// </summary>
public enum ProjectHealthLevel
{
    /// <summary>
    /// Project is exceeding expectations with no issues
    /// </summary>
    Excellent,

    /// <summary>
    /// Project is on track with minor or no issues
    /// </summary>
    Good,

    /// <summary>
    /// Project has significant issues that require attention
    /// </summary>
    Warning,

    /// <summary>
    /// Project has severe issues that require immediate action
    /// </summary>
    Critical
}
