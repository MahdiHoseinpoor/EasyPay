using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Report
{
    public record TransferMoneyRequest(Guid SourceAccountId, string DestinationAccountNumber, decimal Amount, string? Description);
    public record TransactionRequestMetadata(string IpAddress, string UserAgent);
}
