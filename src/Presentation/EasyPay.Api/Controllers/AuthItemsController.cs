using Asp.Versioning;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement;
using EasyPay.Application.Common;
using EasyPay.Application.Queries.AccountManagement.AccountEntity;
using EasyPay.Application.Queries.AccountManagement.AccountTypeDocumentRequirementEntity;
using EasyPay.Application.Queries.Identity.AuthItemEntity;
using EasyPay.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyPay.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth-items/")]
    [ApiController]
    
    public class AuthItemsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> getAllAuthItems()
        {
            var query = new GetAllAuthItemsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
