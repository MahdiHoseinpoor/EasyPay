using EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.DeleteAccount;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount;
using EasyPay.Application.Queries.AccountManagement.AccountEntity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of accounts. 
        /// </summary>
        /// <param name="pageIndex">The page index to retrieve.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>A paginated list of accounts.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllAccounts([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            var query = new GetAllAccountsQuery { PageIndex = pageIndex, PageSize = pageSize };
            var result = await _mediator.Send(query);

            return result.Match<ActionResult>(
                success => Ok(success),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Gets a specific account by its unique ID.
        /// </summary>
        /// <param name="id">The account ID.</param>
        /// <returns>The requested account data.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            var query = new GetAccountByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return result.Match<ActionResult>(
                success => Ok(success),
                failure => NotFound(failure)
            );
        }

        /// <summary>
        /// Creates a new account for the authenticated user.
        /// </summary>
        /// <param name="command">The data for the new account.</param>
        /// <returns>The ID of the newly created account.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                accountId => CreatedAtAction(nameof(GetAccountById), new { id = accountId }, accountId),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Updates an existing account. (Note: Handler logic is currently placeholder).
        /// </summary>
        /// <param name="id">The ID of the account to update.</param>
        /// <param name="command">The update data.</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command)
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
        /// Deletes an account by its ID.
        /// </summary>
        /// <param name="id">The ID of the account to delete.</param>
        /// <param name="isHardDelete">A flag to indicate if a hard delete should be performed (requires special permissions).</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(Guid id, [FromQuery] bool isHardDelete = false)
        {
            var command = new DeleteAccountCommand { Id = id, IsHardDelete = isHardDelete };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }
    }
}