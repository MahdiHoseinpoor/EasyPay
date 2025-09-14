using Asp.Versioning;
using EasyPay.Application.Commands.Identity.RoleEntity.CreateRole;
using EasyPay.Application.Commands.Identity.RoleEntity.UpdateRolePermissions;
using EasyPay.Application.Common;
using EasyPay.Application.Queries.Identity.RoleEntity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = SystemRoles.SuperAdmin)]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a list of all roles.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return result.Match<ActionResult>(
                Ok,
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Creates a new role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Match<ActionResult>(
                role => CreatedAtAction(nameof(GetAllRoles), new { id = role.Id }, role),
                failure => failure.code == 409 ? Conflict(failure) : BadRequest(failure)
            );
        }

        /// <summary>
        /// Gets all system permissions for a specific role.
        /// </summary>
        /// <param name="id">The Role ID.</param>
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetRolePermissions(string id)
        {
            var result = await _mediator.Send(new GetRolePermissionsQuery { RoleId = id });
            return result.Match<ActionResult>(
                Ok,
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }

        /// <summary>
        /// Updates the permissions for a specific role.
        /// </summary>
        /// <param name="id">The Role ID.</param>
        /// <param name="command">The list of selected permissions.</param>
        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdateRolePermissions(string id, [FromBody] UpdateRolePermissionsCommand command)
        {
            if (id != command.RoleId)
            {
                command.RoleId = id;
            }

            var result = await _mediator.Send(command);
            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }
    }
}