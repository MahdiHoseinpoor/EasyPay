using EasyPay.Shared.Enums.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.CreateUtilityBill
{
    public class CreateUtilityBillCommand : IRequest<Result<Guid>>
    {
        public UtilityType UtilityType { get; set; }

        public string BillNumber { get; set; }
    }
}
