using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement;
using EasyPay.Application.Common;
using EasyPay.Application.Queries.AccountManagement.AccountTypeDocumentRequirementEntity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [Route("api/account-types/{accountTypeId}/document-requirements")]
    [ApiController]
    [Authorize(Roles = SystemRoles.SuperAdmin)]
    public class AccountTypeDocumentRequirementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountTypeDocumentRequirementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Adds a new document requirement to an account type.
        /// </summary>
        /// <param name="accountTypeId">The ID of the account type.</param>
        /// <param name="command">The requirement details.</param>
        /// <returns>The ID of the newly created requirement.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateDocumentRequirement(int accountTypeId, [FromBody] CreateAccountTypeDocumentRequirementCommand command)
        {
            command.AccountTypeId = accountTypeId;

            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                requirementId => CreatedAtAction(null, new { accountTypeId = accountTypeId, id = requirementId }, requirementId),
                failure => failure.code == 409 ? Conflict(failure) : BadRequest(failure)
            );
        }

        /// <summary>
        /// Gets all document requirements for a specific account type.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRequirements(int accountTypeId)
        {
            var query = new GetAllAccountTypeDocumentRequirementsQuery { AccountTypeId = accountTypeId };
            var result = await _mediator.Send(query);

            return result.Match<ActionResult>(
                Ok,
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Deletes a document requirement from an account type.
        /// </summary>
        /// <param name="accountTypeId">The ID of the account type.</param>
        /// <param name="requirementId">The ID of the requirement to delete.</param>
        [HttpDelete("{requirementId}")]
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

            return result.Match<ActionResult>(
                () => NoContent(),
                failure => failure is NotFoundError ? NotFound(failure) : BadRequest(failure)
            );
        }
    }
}