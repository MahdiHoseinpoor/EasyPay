using EasyPay.Shared.Models.Report;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Payment.PayBill
{
    public class PayBillCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid BillId { get; set; }
        public Guid FromAccountId { get; set; }

        [JsonIgnore]
        public TransactionRequestMetadata? RequestMetadata { get; set; }
    }
}