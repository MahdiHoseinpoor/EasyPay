using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.Entites.AccountManagement
{
    public class BankCard:EntityBase<int>
    {
        public string Title { get; set; }
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }
        public string InternationalBankAccountNumber { get; set; }
    }
}
