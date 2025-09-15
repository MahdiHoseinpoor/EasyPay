using EasyPay.Shared.Models.Report;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class WithdrawToBankCardCommand : IRequest<Result<Guid>>
    {
        public Guid AccountId { get; set; }
        public int DestinationBankCardId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public TransactionRequestMetadata? RequestMetadata { get; set; }
    }
}