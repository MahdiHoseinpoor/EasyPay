using EasyPay.Application.Commands.Identity.AuthItemValueEntity.CreateAuthItemValue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Submits a document or value for a specific authentication item.
        /// </summary>
        /// <remarks>
        /// For file uploads, the client should first upload the file to a storage endpoint,
        /// receive a URL/identifier, and then submit that identifier in the 'Value' field of this request.
        /// </remarks>
        /// <param name="command">The command containing the auth item data.</param>
        /// <returns>The ID of the newly created submission record.</returns>
        [HttpPost("auth-item-values")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitAuthItemValue([FromBody] SubmitAuthItemValueCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                authItemValueId => CreatedAtAction(nameof(SubmitAuthItemValue), new { id = authItemValueId }, authItemValueId),
                failure => BadRequest(failure)
            );
        }
    }
}