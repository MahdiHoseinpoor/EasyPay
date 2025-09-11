using EasyPay.Application.Commands.Identity.LoginCommand;
using EasyPay.Application.Commands.Identity.NaturalUserRegisterPhoneCommand;
using EasyPay.Application.Commands.Identity.NaturalUserRegisterVerifyPhoneCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">The user's login credentials.</param>
        /// <returns>A JWT token upon successful authentication.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                loginResponse => Ok(loginResponse),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Step 1 of registration: Submits a phone number to receive a verification code.
        /// </summary>
        /// <param name="request">The request containing the user's phone number.</param>
        [HttpPost("register/request-phone-verification")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RequestPhoneVerification([FromBody] RegisterPhoneRequest request)
        {
            var command = new NaturalUserRegisterPhoneCommand { Phone = request.phone };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => Ok(new { Message = "Verification code has been sent." }),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Step 2 of registration: Verifies a phone number using the received code, creates the user, and returns a token.
        /// </summary>
        /// <param name="request">The request containing the phone number and verification code.</param>
        /// <returns>A token to be used for completing the user profile.</returns>
        [HttpPost("register/verify-phone")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(NaturalUserRegisterVerifyPhoneResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> VerifyPhone([FromBody] NaturalUserRegisterVerifyPhoneRequest request)
        {
            var command = new NaturalUserRegisterVerfiyPhoneCommand { Phone = request.phone, Code = request.code };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                response => Ok(response),
                failure => failure.code == 409 ? Conflict(failure) : BadRequest(failure)
            );
        }
    }
}