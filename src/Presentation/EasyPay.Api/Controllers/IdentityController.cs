using Asp.Versioning;
using EasyPay.Application.Commands.Identity.AuthenticateWithPhone;
using EasyPay.Application.Commands.Identity.NaturalUserEntity.CreateNaturalUser;
using EasyPay.Application.Commands.Identity.NaturalUserRegisterPhoneCommand;
using EasyPay.Application.Commands.Identity.VerifyPasswordCommand;
using EasyPay.Shared.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// <param name="request">The user's VerifyPassword credentials.</param>
        /// <returns>A JWT token upon successful authentication.</returns>
        [HttpPost("VerifyPassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(VerifyPasswordResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
        {
            var command = new VerifyPasswordCommand
            {
                Username = request.Username,
                Password = request.Password,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                VerifyPasswordResponse => Ok(VerifyPasswordResponse),
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
        [ProducesResponseType(typeof(AuthenticateWithPhoneResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> VerifyPhone([FromBody] AuthenticateWithPhoneRequest request)
        {
            var command = new AuthenticateWithPhoneCommand { Phone = request.phone, Code = request.code };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                response => Ok(response),
                failure => failure.code == 409 ? Conflict(failure) : BadRequest(failure)
            );
        }
        /// <summary>
        /// Step 3 of registration: Completes the user profile with personal details.
        /// This must be called after phone verification using the token provided.
        /// </summary>
        /// <param name="command">The user's personal information.</param>
        /// <returns>An HTTP status code indicating the result.</returns>
        [HttpPost("register/complete-profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
        {
            var command = new CompleteNaturalUserProfileCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                NationalCode = request.NationalCode,
                BirthDate = request.BirthDate,
                FatherName = request.FatherName,
                Gender = request.Gender
            };
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                () => Ok(new { Message = "User profile completed successfully." }),
                failure => BadRequest(failure)
            );
        }
    }
}