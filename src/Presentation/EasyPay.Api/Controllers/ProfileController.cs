using Asp.Versioning;
using EasyPay.Application.Commands.Identity.AuthItemValueEntity.CreateAuthItemValue;
using EasyPay.Application.Queries.AccountManagement.AccountEntity;
using EasyPay.Application.Queries.AccountManagement.BankCardEntity;
using EasyPay.Application.Queries.Identity.AuthItemValueEntity;
using EasyPay.Application.Queries.Report.TransactionEntity;
using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Identity;
using EasyPay.Shared.DTOs.Report;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Submits a document or value for a specific authentication item.
        /// </summary>
        /// <remarks>
        /// For file uploads, the client should first upload the file to a storage endpoint,
        /// receive a URL/identifier, and then submit that identifier in the 'Value' field of this request.
        /// </remarks>
        /// <param name="command">The command containing the auth item data.</param>
        /// <returns>The ID of the newly created submission record.</returns>
        [HttpPost("auth-item-values")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitAuthItemValue([FromBody] SubmitAuthItemValueCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match<ActionResult>(
                authItemValueId => CreatedAtAction(nameof(SubmitAuthItemValue), new { id = authItemValueId }, authItemValueId),
                failure => BadRequest(failure)
            );
        }

        /// <summary>
        /// Gets all accounts owned by the current authenticated user.
        /// </summary>
        [HttpGet("accounts")]
        [ProducesResponseType(typeof(List<EasyPay.Shared.DTOs.AccountManagement.AccountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAccounts()
        {
            var query = new GetMyAccountsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets the transaction history for one of the user's accounts.
        /// </summary>
        /// <param name="accountId">The ID of the account to retrieve history for.</param>
        /// <param name="pageIndex">The page index for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        [HttpGet("accounts/{accountId}/transactions")]
        [ProducesResponseType(typeof(IPagedList<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTransactionHistory(Guid accountId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            var query = new GetMyTransactionHistoryQuery { AccountId = accountId, PageIndex = pageIndex, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets all documents and auth items submitted by the current authenticated user.
        /// </summary>
        [HttpGet("auth-item-values")]
        [ProducesResponseType(typeof(List<AuthItemValueDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMySubmittedDocuments()
        {
            var query = new GetMySubmittedDocumentsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets all bank cards owned by the current authenticated user.
        /// </summary>
        [HttpGet("bank-cards")]
        [ProducesResponseType(typeof(List<BankCardDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyBankCards()
        {
            var query = new GetMyBankCardsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}