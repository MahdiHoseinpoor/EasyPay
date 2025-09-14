using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAllAccountsQuery : IRequest<Result<IPagedList<AccountDto>>>
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}