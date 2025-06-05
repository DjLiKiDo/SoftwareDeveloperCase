using Microsoft.AspNetCore.Authorization;

namespace SoftwareDeveloperCase.Api.Authorization.Requirements;

/// <summary>
/// Authorization requirement for project access validation
/// </summary>
public class ProjectAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAccessRequirement"/> class
    /// </summary>
    /// <param name="operation">The operation being performed on the project</param>
    public ProjectAccessRequirement(string operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets the operation being performed
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Available project operations for authorization
    /// </summary>
    public static class Operations
    {
        /// <summary>Read project information</summary>
        public const string Read = "Read";
        /// <summary>Create new project</summary>
        public const string Create = "Create";
        /// <summary>Update project information</summary>
        public const string Update = "Update";
        /// <summary>Delete project</summary>
        public const string Delete = "Delete";
        /// <summary>Manage project tasks</summary>
        public const string ManageTasks = "ManageTasks";
    }
}
