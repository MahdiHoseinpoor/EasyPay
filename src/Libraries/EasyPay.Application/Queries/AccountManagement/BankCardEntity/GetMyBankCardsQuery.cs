
using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.BankCardEntity
{
    public class GetMyBankCardsQuery : IRequest<Result<List<BankCardDto>>>
    {
    }
}
