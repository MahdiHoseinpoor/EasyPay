using EasyPay.Application.DTOs.AccountManagement;
using MediatR;

namespace EasyPay.Application.Queries.AccountManagement.BankCardEntity
{
    public class GetBankCardByIdQuery : IRequest<Result<BankCardDto>>
    {
        public int Id { get; set; }
    }
}