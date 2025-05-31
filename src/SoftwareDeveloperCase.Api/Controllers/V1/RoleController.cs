using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.InsertRole;
using System.Net;

namespace SoftwareDeveloperCase.Api.Controllers.V1;

/// <summary>
/// Controller for managing roles and role-related operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling commands and queries.</param>
    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="command">The command containing role information to create.</param>
    /// <returns>The ID of the created role.</returns>
    [HttpPost(Name = "InsertRole")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> InsertRole([FromBody] InsertRoleCommand command)
    {
        return await _mediator.Send(command);
    }

    /// <summary>
    /// Assigns a permission to a role.
    /// </summary>
    /// <param name="command">The command containing role and permission assignment information.</param>
    /// <returns>The ID of the role permission assignment.</returns>
    [HttpPost("AssignPermission", Name = "AssignPermission")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<Guid>> AssignPermission([FromBody] AssignPermissionCommand command)
    {
        return await _mediator.Send(command);
    }
}
