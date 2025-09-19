using EasyPay.Shared.Models.Report;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class RequestGatewayDepositCommand : IRequest<Result<GatewayDepositResponse>>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string GatewayName { get; set; }
    }
}
