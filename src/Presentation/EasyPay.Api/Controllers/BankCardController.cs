using EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.DeleteBankCard;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard;
using EasyPay.Application.Queries.AccountManagement.BankCardEntity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/bank-cards")]
    [ApiController]
    public class BankCardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BankCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a specific bank card by its ID.
        /// </summary>
        /// <param name="id">The bank card ID.</param>
        /// <returns>The requested bank card data.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBankCardById(int id)
        {
            var query = new GetBankCardByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return result.Match<ActionResult>(
                success => Ok(success),
                failure => NotFound(failure)
            );
        }

        /// <summary>
        /// Creates a new bank card.
        /// </summary>
        /// <param name="command">The data for the new bank card.</param>
        /// <returns>The ID of the newly created bank card.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBankCard([FromBody] CreateBankCardCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                bankCardId => CreatedAtAction(nameof(GetBankCardById), new { id = bankCardId }, bankCardId),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Updates an existing bank card.
        /// </summary>
        /// <param name="id">The ID of the bank card to update.</param>
        /// <param name="command">The update data.</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBankCard(int id, [FromBody] UpdateBankCardCommand command)
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
        /// Deletes a bank card by its ID.
        /// </summary>
        /// <param name="id">The ID of the bank card to delete.</param>
        /// <param name="isHardDelete">Flag for performing a hard delete (requires permissions).</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBankCard(int id, [FromQuery] bool isHardDelete = false)
        {
            var command = new DeleteBankCardCommand { Id = id, IsHardDelete = isHardDelete };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }
    }
}