using MediatR;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class VerifyGatewayDepositCommand : IRequest<Result<string>>
    {
        public string GatewayToken { get; set; }
        public string Status { get; set; }
    }
}