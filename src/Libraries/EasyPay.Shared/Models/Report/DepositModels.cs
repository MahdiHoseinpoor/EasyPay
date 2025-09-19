using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Report
{
    public record GatewayDepositRequest(Guid AccountId, decimal Amount, string GatewayName);
    public record GatewayDepositResponse(string RedirectUrl);
}
