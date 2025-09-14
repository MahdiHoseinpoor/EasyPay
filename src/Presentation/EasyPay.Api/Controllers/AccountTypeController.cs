using Asp.Versioning;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.DeleteAccountType;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.UpdateAccountType;
using EasyPay.Application.Queries.AccountManagement.AccountTypeEntity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/account-types")]
    [ApiController]
    [Authorize]
    public class AccountTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a specific account type by its ID.
        /// </summary>
        /// <param name="id">The account type ID.</param>
        /// <returns>The requested account type data.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccountTypeById(int id)
        {
            var query = new GetAccountTypeByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return result.Match<ActionResult>(
                success => Ok(success),
                failure => NotFound(failure)
            );
        }

        /// <summary>
        /// Creates a new account type.
        /// </summary>
        /// <param name="command">The data for the new account type.</param>
        /// <returns>The ID of the newly created account type.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccountType([FromBody] CreateAccountTypeCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                accountTypeId => CreatedAtAction(nameof(GetAccountTypeById), new { id = accountTypeId }, accountTypeId),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Updates an existing account type.
        /// </summary>
        /// <param name="id">The ID of the account type to update.</param>
        /// <param name="command">The update data.</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAccountType(int id, [FromBody] UpdateAccountTypeCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Route ID and command ID do not match.");
            }

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }

        /// <summary>
        /// Deletes an account type by its ID.
        /// </summary>
        /// <param name="id">The ID of the account type to delete.</param>
        /// <param name="isHardDelete">Flag for performing a hard delete (requires permissions).</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccountType(int id, [FromQuery] bool isHardDelete = false)
        {
            var command = new DeleteAccountTypeCommand { Id = id, IsHardDelete = isHardDelete };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }
    }
}