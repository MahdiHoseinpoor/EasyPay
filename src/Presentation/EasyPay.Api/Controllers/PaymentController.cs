using Asp.Versioning;
using EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction;
using EasyPay.Common;

using EasyPay.Common.Errors.Business;
using EasyPay.Shared.Models.Report;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public PaymentController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }



        /// <summary>
        /// Initiates a deposit into an account via an external payment gateway.
        /// </summary>
        [HttpPost("deposit/gateway")]
        [Authorize]
        public async Task<IActionResult> RequestGatewayDeposit([FromBody] GatewayDepositRequest request)
        {
            var command = new RequestGatewayDepositCommand
            {
                AccountId = request.AccountId,
                Amount = request.Amount,
                GatewayName = request.GatewayName
            };

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                response => Ok(response),
                error => BadRequest(error)
            );
        }

        /// <summary>
        /// Callback endpoint for payment gateways to redirect the user to.
        /// </summary>
        [HttpGet("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GatewayCallback([FromQuery] string Authority, [FromQuery] string Status)
        {
            var command = new VerifyGatewayDepositCommand { GatewayToken = Authority, Status = Status };
            var result = await _mediator.Send(command);
            var clientUrl = _configuration["ClientAppSettings:BaseUrl"] ?? "http://localhost:5038";

            return result.Match<ActionResult>(
                successMessage => Redirect($"{clientUrl}/deposit-result?status=success"),
                error => Redirect($"{clientUrl}/deposit-result?status=failed&message={Uri.EscapeDataString(error.message)}")
            );
        }
    }
}