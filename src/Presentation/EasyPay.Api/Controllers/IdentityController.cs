using Azure.Core;
using EasyPay.Application.Commands.Identity.LoginCommand;
using EasyPay.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        IMediator _mediator;
        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/Login")]
        public async Task<ActionResult> Login(LoginRequest request)
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
                success => Ok(success),
                failure => BadRequest(failure)
            );
        }
        [HttpPost("/Register")]
        public async Task<ActionResult> Register()
        {

        }
    }
}
