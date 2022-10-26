using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.User.Commands.DeleteUser;
using SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions;
using System.Net;

namespace SoftwareDeveloperCase.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "InsertUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Guid>> InsertUser([FromBody] InsertUserCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut(Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

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

        [HttpGet("GetUserPermissions/{userId}", Name = "GetUserPermissions")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetUserPermissions(Guid userId)
        {
            var query = new GetUserPermissionsQuery(userId);
            var userPermissions = await _mediator.Send(query);

            return Ok(userPermissions);
        }

        [HttpPost("AssignRole", Name = "AssignRole")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Guid>> AssignRole([FromBody] AssignRoleCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
