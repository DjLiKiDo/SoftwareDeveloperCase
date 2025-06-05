using Microsoft.AspNetCore.Authorization;

namespace SoftwareDeveloperCase.Api.Authorization.Attributes;

/// <summary>
/// Authorization attribute for team operations
/// </summary>
public class AuthorizeTeamAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeTeamAttribute"/> class
    /// </summary>
    /// <param name="operation">The team operation</param>
    public AuthorizeTeamAttribute(string operation)
    {
        Policy = $"Team{operation}";
    }
}

/// <summary>
/// Authorization attribute for project operations
/// </summary>
public class AuthorizeProjectAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeProjectAttribute"/> class
    /// </summary>
    /// <param name="operation">The project operation</param>
    public AuthorizeProjectAttribute(string operation)
    {
        Policy = $"Project{operation}";
    }
}

/// <summary>
/// Authorization attribute for task operations
/// </summary>
public class AuthorizeTaskAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeTaskAttribute"/> class
    /// </summary>
    /// <param name="operation">The task operation</param>
    public AuthorizeTaskAttribute(string operation)
    {
        Policy = $"Task{operation}";
    }
}
