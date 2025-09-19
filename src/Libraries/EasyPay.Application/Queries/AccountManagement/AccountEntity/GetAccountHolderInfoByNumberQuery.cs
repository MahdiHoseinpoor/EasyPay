using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAccountHolderInfoByNumberQuery : IRequest<Result<AccountHolderDto>>, IAuthorizableRequest<Result<AccountHolderDto>>
    {
        public string AccountNumber { get; set; }
        public string RequiredPermission => Permissions.Accounts.View; 
    }
}