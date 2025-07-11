using EasyPay.Domain.Entites.Identity;
using EasyPay.Domain.Enums.AccountManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPay.Domain.Entites.AccountManagement
{
    public class Account : EntityBase
    {
        public int AccountTypeId { get; set; }
        public string Title { get; set; }
        public string AccountNumber { get; set; }

        [ForeignKey(nameof(AccountTypeId))]
        public virtual AccountType AccountType { get; set; }

        [Required]
        public string OwnerUserId { get; set; }

        [ForeignKey(nameof(OwnerUserId))]
        public virtual ApplicationUser OwnerUser { get; set; }

        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public decimal CurrentBalance { get; set; } = 0;

        public DateTime OpeningDate { get; set; } = DateTime.Now;

        public DateTime? ClosingDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

    }
}
