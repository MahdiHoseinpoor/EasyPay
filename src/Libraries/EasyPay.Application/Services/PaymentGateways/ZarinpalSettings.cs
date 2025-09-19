using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services.PaymentGateways
{
    public class ZarinpalSettings
    {
        public const string SectionName = "PaymentGateways:Zarinpal";
        public string MerchantId { get; set; }
        public bool IsSandbox { get; set; }
    }
}
