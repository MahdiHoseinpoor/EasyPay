using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;
using System.Collections.Generic;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetMyAccountsQuery : IRequest<Result<List<AccountDto>>>
    {
        // No parameters needed as it's always for the current user.
    }
}