using EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction;
using EasyPay.Common.Errors.Business;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Deposits money into a specified account.
        /// </summary>
        /// <param name="command">The deposit details.</param>
        /// <returns>The ID of the created transaction record.</returns>
        [HttpPost("deposit")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deposit([FromBody] DepositMoneyCommand command)
        {
            // Populate metadata from the request context
            command.RequestMetadata = new TransactionRequestMetadata(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers["User-Agent"].ToString()
            );

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                transactionId => CreatedAtAction(null, new { id = transactionId }, transactionId),
                failure => failure switch
                {
                    NotFoundError => NotFound(failure),
                    { code: 403 } => Forbid(),
                    _ => BadRequest(failure)
                }
            );
        }

        /// <summary>
        /// Withdraws money from a specified account.
        /// </summary>
        /// <param name="command">The withdrawal details.</param>
        /// <returns>The ID of the created transaction record.</returns>
        [HttpPost("withdraw")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawMoneyCommand command)
        {

            command.RequestMetadata = new TransactionRequestMetadata(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers["User-Agent"].ToString()
            );

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                transactionId => CreatedAtAction(null, new { id = transactionId }, transactionId),
                failure => failure switch
                {
                    NotFoundError => NotFound(failure),
                    { code: 403 } => Forbid(),
                    _ => BadRequest(failure)
                }
            );
        }
        /// <summary>
        /// Transfers money from one user's account to another.
        /// </summary>
        /// <param name="command">The transfer details.</param>
        /// <returns>The ID of the created outgoing transaction record.</returns>
        [HttpPost("transfer")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Transfer([FromBody] TransferMoneyCommand command)
        {
            command.RequestMetadata = new TransactionRequestMetadata(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                HttpContext.Request.Headers["User-Agent"].ToString()
            );

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                transactionId => CreatedAtAction(null, new { id = transactionId }, transactionId),
                failure => failure switch
                {
                    NotFoundError => NotFound(failure),
                    AuthorizationError => Forbid(), // Use Forbid() for 403
                    _ => BadRequest(failure)
                }
            );
        }
    }
}