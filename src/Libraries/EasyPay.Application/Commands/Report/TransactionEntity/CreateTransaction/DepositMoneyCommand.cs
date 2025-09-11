using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class DepositMoneyCommand : IRequest<Result<Guid>>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        [JsonIgnore] 
        public TransactionRequestMetadata? RequestMetadata { get; set; }
    }

    public record TransactionRequestMetadata(string IpAddress, string UserAgent);
}