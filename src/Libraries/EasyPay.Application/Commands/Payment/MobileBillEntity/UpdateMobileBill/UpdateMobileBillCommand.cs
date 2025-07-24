using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.UpdateMobileBill
{
    public class UpdateMobileBillCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public UpdateMobileBillCommand()
        {
            
        }
    }
}
