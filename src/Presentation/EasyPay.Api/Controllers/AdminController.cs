using Asp.Versioning;
using EasyPay.Application.Commands.Identity.AuthItemValueEntity.ApproveAuthItemValue;
using EasyPay.Application.Commands.Identity.AuthItemValueEntity.RejectAuthItemValue;
using EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetPendingSubmissions;
using EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetSubmissionDetail;
using EasyPay.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = SystemRoles.SuperAdmin)]
    public class AdminController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of pending document verifications.
        /// </summary>
        [HttpGet("verifications/pending")]
        public async Task<IActionResult> GetPendingVerifications([FromQuery] GetPendingSubmissionsQuery query)
        {
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Gets the details of a single document verification submission.
        /// </summary>
        [HttpGet("verifications/{id}")]
        public async Task<IActionResult> GetVerificationDetail(long id)
        {
            var result = await _mediator.Send(new GetSubmissionDetailQuery { Id = id });
            return HandleResult(result);
        }

        /// <summary>
        /// Approves a document verification submission.
        /// </summary>
        [HttpPost("verifications/{id}/approve")]
        public async Task<IActionResult> ApproveVerification(long id)
        {
            var result = await _mediator.Send(new ApproveAuthItemValueCommand { AuthItemValueId = id });
            return HandleResult(result);
        }

        /// <summary>
        /// Rejects a document verification submission.
        /// </summary>
        [HttpPost("verifications/{id}/reject")]
        public async Task<IActionResult> RejectVerification(long id, [FromBody] RejectAuthItemValueCommand command)
        {
            command.AuthItemValueId = id;
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}