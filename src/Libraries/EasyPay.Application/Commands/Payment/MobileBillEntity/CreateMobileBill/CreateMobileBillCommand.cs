using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill
{
    public class CreateMobileBillCommand : IRequest<Result<Guid>>
    {
        public string PhoneNumber { get; set; }
    }
}
