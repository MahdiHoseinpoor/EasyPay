using System;

namespace EasyPay.Application.Services.PaymentGateways
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGatewayService Create(string gatewayName);
    }
}