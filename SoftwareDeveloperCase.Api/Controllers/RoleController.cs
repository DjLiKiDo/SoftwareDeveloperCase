using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole;
using System.Net;

namespace SoftwareDeveloperCase.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "InsertRole")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Guid>> InsertRole([FromBody] InsertRoleCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("AssignPermission", Name = "AssignPermission")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Guid>> AssignPermission([FromBody] AssignPermissionCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
