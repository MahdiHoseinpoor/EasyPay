using Asp.Versioning;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement;
using EasyPay.Application.Common;
using EasyPay.Application.Queries.AccountManagement.AccountTypeDocumentRequirementEntity;
using EasyPay.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/account-types/{accountTypeId}/document-requirements")]
    [ApiController]
    // Allow authenticated users to view requirements, but only admins to change them.
    [Authorize]
    public class AccountTypeDocumentRequirementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountTypeDocumentRequirementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all document requirements for a specific account type.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<EasyPay.Shared.DTOs.AccountManagement.AccountTypeDocumentRequirementDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllRequirements(int accountTypeId)
        {
            var query = new GetAllAccountTypeDocumentRequirementsQuery { AccountTypeId = accountTypeId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Adds a new document requirement to an account type. (Admin Only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = SystemRoles.SuperAdmin)] // Secure this action
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateDocumentRequirement(int accountTypeId, [FromBody] CreateAccountTypeDocumentRequirementCommand command)
        {
            command.AccountTypeId = accountTypeId;

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return result.error.code == 409 ? Conflict(result) : BadRequest(result);

            return CreatedAtAction(nameof(GetAllRequirements), new { accountTypeId = accountTypeId, id = result.Value }, result);
        }

        /// <summary>
        /// Deletes a document requirement from an account type. (Admin Only)
        /// </summary>
        [HttpDelete("{requirementId}")]
        [Authorize(Roles = SystemRoles.SuperAdmin)] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocumentRequirement(int accountTypeId, int requirementId)
        {
            var command = new DeleteAccountTypeDocumentRequirementCommand
            {
                AccountTypeId = accountTypeId,
                RequirementId = requirementId
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return result.error is NotFoundError ? NotFound(result) : BadRequest(result);

            return NoContent();
        }
    }
}