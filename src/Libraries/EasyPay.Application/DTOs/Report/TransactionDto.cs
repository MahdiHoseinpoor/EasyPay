using EasyPay.Domain.Enums.Report;
using System;

namespace EasyPay.Application.DTOs.Report
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceId { get; set; }
        public string Description { get; set; }
        public TransactionStatus Status { get; set; }
    }
}