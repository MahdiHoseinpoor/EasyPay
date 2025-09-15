using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Report
{
    public record WithdrawToBankCardRequest(Guid AccountId, int DestinationBankCardId, decimal Amount, string? Description);
}
