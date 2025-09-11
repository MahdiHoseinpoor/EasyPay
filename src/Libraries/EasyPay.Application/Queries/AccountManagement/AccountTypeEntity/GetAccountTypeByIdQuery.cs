using EasyPay.Application.DTOs.AccountManagement;
using MediatR;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeEntity
{
    public class GetAccountTypeByIdQuery : IRequest<Result<AccountTypeDto>>
    {
        public int Id { get; set; }
    }
}