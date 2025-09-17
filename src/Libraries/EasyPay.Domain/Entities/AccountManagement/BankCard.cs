using EasyPay.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.Entities.AccountManagement
{
    public class BankCard:EntityBase<int>
    {
        public string OwnerUserId { get; set; }
        public virtual NaturalUser OwnerUser { get; set; }
        public string Title { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string InternationalBankAccountNumber { get; set; } = string.Empty;

        [SetsRequiredMembers]
        public BankCard()
        {
            
        }
    }
}
