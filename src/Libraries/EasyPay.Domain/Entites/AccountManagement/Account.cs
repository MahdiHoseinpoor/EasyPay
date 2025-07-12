using EasyPay.Domain.Entites.Identity;
using EasyPay.Domain.Entites.Report;
using EasyPay.Domain.Enums.AccountManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPay.Domain.Entites.AccountManagement
{
    public class Account : EntityBase<Guid>
    {
        public int AccountTypeId { get; set; }
        public string Title { get; set; }
        public string AccountNumber { get; set; }

        public virtual AccountType AccountType { get; set; }

        public string OwnerUserId { get; set; }

        public virtual ApplicationUser OwnerUser { get; set; }

        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public decimal CurrentBalance { get; set; } = 0;

        public DateTime OpeningDate { get; set; } = DateTime.Now;

        public DateTime? ClosingDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}
