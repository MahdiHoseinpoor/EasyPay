using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services.PaymentGateways
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGatewayService> _gatewayServices;

        public PaymentGatewayFactory(IEnumerable<IPaymentGatewayService> gatewayServices)
        {
            _gatewayServices = gatewayServices;
        }

        public IPaymentGatewayService Create(string gatewayName)
        {
            var service = _gatewayServices.FirstOrDefault(s => s.GatewayName.Equals(gatewayName, StringComparison.OrdinalIgnoreCase));

            if (service == null)
            {
                throw new NotSupportedException($"The payment gateway '{gatewayName}' is not supported.");
            }

            return service;
        }
    }
}
