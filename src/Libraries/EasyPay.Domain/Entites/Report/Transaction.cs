using EasyPay.Domain.Entites.AccountManagement;
using EasyPay.Domain.Enums.Report;
using EasyPay.Domain.ValueObjects.Report;
using System.Transactions;
using TransactionStatus = EasyPay.Domain.Enums.Report.TransactionStatus;

namespace EasyPay.Domain.Entites.Report
{
    public class Transaction:EntityBase<Guid>
    {
        public string AccountId { get; set; }

        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string ReferenceId { get; set; }

        public string Description { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public TransactionMetadata TransactionMetadata { get; set; }

        public virtual Account Account { get; set; }
    }

}
