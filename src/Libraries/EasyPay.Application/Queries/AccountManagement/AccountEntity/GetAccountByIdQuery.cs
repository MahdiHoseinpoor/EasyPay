using EasyPay.Application.DTOs.AccountManagement;
using MediatR;
using System;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAccountByIdQuery : IRequest<Result<AccountDto>>
    {
        public Guid Id { get; set; }
    }
}