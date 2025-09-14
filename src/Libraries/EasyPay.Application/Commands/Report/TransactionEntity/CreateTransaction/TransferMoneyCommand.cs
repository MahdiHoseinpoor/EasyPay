using EasyPay.Shared.Models.Report;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class TransferMoneyCommand : IRequest<Result<Guid>>
    {
        public Guid SourceAccountId { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public TransactionRequestMetadata? RequestMetadata { get; set; }
    }
}