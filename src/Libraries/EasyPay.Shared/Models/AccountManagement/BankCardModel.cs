using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.AccountManagement
{
    public record CreateBankCardRequest(string Title, string CardNumber, string AccountNumber, string InternationalBankAccountNumber);
}
