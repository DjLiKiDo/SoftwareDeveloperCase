using Microsoft.AspNetCore.Authorization;

namespace SoftwareDeveloperCase.Api.Authorization.Requirements;

/// <summary>
/// Authorization requirement for task access validation
/// </summary>
public class TaskAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAccessRequirement"/> class
    /// </summary>
    /// <param name="operation">The operation being performed on the task</param>
    public TaskAccessRequirement(string operation)
    {
        Operation = operation;
    }

    /// <summary>
    /// Gets the operation being performed
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Available task operations for authorization
    /// </summary>
    public static class Operations
    {
        /// <summary>Read task information</summary>
        public const string Read = "Read";
        /// <summary>Create new task</summary>
        public const string Create = "Create";
        /// <summary>Update task information</summary>
        public const string Update = "Update";
        /// <summary>Delete task</summary>
        public const string Delete = "Delete";
        /// <summary>Assign task to user</summary>
        public const string Assign = "Assign";
        /// <summary>Update task status</summary>
        public const string UpdateStatus = "UpdateStatus";
        /// <summary>Add comment to task</summary>
        public const string AddComment = "AddComment";
    }
}
