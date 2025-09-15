using Asp.Versioning;
using EasyPay.Application.Commands.Payment.PayBill;
using EasyPay.Common.Errors.Business;
using EasyPay.Shared.Models.Report;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/bills")]
    [ApiController]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BillsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record PayBillRequest(Guid FromAccountId);

        /// <summary>
        /// Pays a specific bill from a user's account.
        /// </summary>
        /// <param name="billId">The ID of the bill to pay.</param>
        /// <param name="request">The request containing the source account ID.</param>
        /// <returns>The ID of the resulting payment transaction.</returns>
        [HttpPost("{billId:guid}/pay")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PayBill([FromRoute] Guid billId, [FromBody] PayBillRequest request)
        {
            var command = new PayBillCommand
            {
                BillId = billId,
                FromAccountId = request.FromAccountId,
                RequestMetadata = new TransactionRequestMetadata(
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers["User-Agent"].ToString()
                )
            };

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                transactionId => CreatedAtAction(null, new { id = transactionId }, transactionId),
                failure => failure switch
                {
                    NotFoundError => NotFound(failure),
                    AuthorizationError => Forbid(),
                    _ => BadRequest(failure)
                }
            );
        }
    }
}