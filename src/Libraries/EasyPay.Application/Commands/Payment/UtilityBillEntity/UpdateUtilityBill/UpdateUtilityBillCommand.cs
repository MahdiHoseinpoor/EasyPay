using EasyPay.Domain.Enums.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.UpdateUtilityBill
{
    public class UpdateUtilityBillCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public UtilityType UtilityType { get; set; }

        public string BillNumber { get; set; }
    }
}
