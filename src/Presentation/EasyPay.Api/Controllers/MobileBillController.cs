using Asp.Versioning;
using EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill;
using EasyPay.Shared.Models.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/mobile-bill")]
    [ApiController]
    [Authorize]
    public class MobileBillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MobileBillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Inquires and creates a mobile bill record for payment.
        /// </summary>
        /// <param name="command">The request containing the phone number.</param>
        /// <returns>The ID of the newly created bill record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateMobileBill([FromBody] CreateMobileBillRequest request)
        {
            var command = new CreateMobileBillCommand()
            {
                PhoneNumber = request.PhoneNumber
            };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                billId => CreatedAtAction(null, new { id = billId }, billId),
                failure => failure switch
                {
                    NotFoundError => NotFound(failure),
                    _ => BadRequest(failure)
                }
            );
        }
    }
}