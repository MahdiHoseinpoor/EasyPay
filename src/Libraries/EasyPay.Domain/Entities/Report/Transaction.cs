using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Enums.Report;
using EasyPay.Domain.ValueObjects.Report;
using System.Diagnostics.CodeAnalysis;

namespace EasyPay.Domain.Entities.Report
{
    public class Transaction : EntityBase<Guid>
    {

        private Transaction() { }

        [SetsRequiredMembers]
        public Transaction(string accountId, decimal amount, TransactionType transactionType, string referenceId, TransactionMetadata metadata, string description = null)
        {
            AccountId = accountId;
            Amount = amount;
            TransactionType = transactionType;
            ReferenceId = referenceId;
            Description = description;
            TransactionMetadata = metadata;
            Status = TransactionStatus.Completed;
            TransactionDate = DateTime.UtcNow;
        }

        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceId { get; set; }
        public string Description { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionMetadata TransactionMetadata { get; set; }
        public virtual Account Account { get; set; }
    }