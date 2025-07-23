using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.DeleteBankCard
{
    public class DeleteBankCardCommand:IRequest<Result>
    {
        public int Id { get; set; }
        public bool IsHardDelete { get; set; }
    }
}
