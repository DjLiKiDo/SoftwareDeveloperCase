using Microsoft.AspNetCore.Authorization;

namespace SoftwareDeveloperCase.Api.Authorization.Requirements;

/// <summary>
/// Authorization requirement for team access validation
/// </summary>
public class TeamAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamAccessRequirement"/> class
    /// </summary>
    /// <param name="operation">The operation being performed on the team</param>
    public TeamAccessRequirement(string operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets the operation being performed
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Available team operations for authorization
    /// </summary>
    public static class Operations
    {
        /// <summary>Read team information</summary>
        public const string Read = "Read";
        /// <summary>Create new team</summary>
        public const string Create = "Create";
        /// <summary>Update team information</summary>
        public const string Update = "Update";
        /// <summary>Delete team</summary>
        public const string Delete = "Delete";
        /// <summary>Manage team members</summary>
        public const string ManageMembers = "ManageMembers";
    }
}
