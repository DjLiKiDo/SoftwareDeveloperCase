using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.DeleteUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;
using System.Net;

namespace SoftwareDeveloperCase.Api.Controllers.V1;

/// <summary>
/// Controller for managing users and user-related operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling commands and queries.</param>
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="command">The command containing user information to create.</param>
    /// <returns>The ID of the created user.</returns>
    [HttpPost(Name = "InsertUser")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> InsertUser([FromBody] InsertUserCommand command)
    {
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="command">The command containing updated user information.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut(Name = "UpdateUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateUser([FromBody] UpdateUserCommand command)
    {
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{userId}", Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> DeleteUser(Guid userId)
    {
        var command = new DeleteUserCommand
        {
            Id = userId
        };

        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Retrieves all permissions assigned to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose permissions to retrieve.</param>
    /// <returns>A collection of permissions assigned to the user.</returns>
    [HttpGet("GetUserPermissions/{userId}", Name = "GetUserPermissions")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetUserPermissions(Guid userId)
    {
        var query = new GetUserPermissionsQuery(userId);
        var userPermissions = await _mediator.Send(query);

        return Ok(userPermissions);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="command">The command containing user and role assignment information.</param>
    /// <returns>The ID of the user role assignment.</returns>
    [HttpPost("AssignRole", Name = "AssignRole")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> AssignRole([FromBody] AssignRoleCommand command)
    {
        return await _mediator.Send(command);
    }
}
