using EasyPay.Shared.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services.PaymentGateways
{
    public record PaymentRequest(decimal Amount, string CallbackUrl, string Description, Guid TransactionId, string? Currency = "IRT");
    public record PaymentRequestResult(string RedirectUrl, string Token);
    public record PaymentVerificationRequest(decimal Amount, string Token);
    public record PaymentVerificationResult(string FinalReferenceId);
    public interface IPaymentGatewayService
    {
        string GatewayName { get; }
        Task<Result<PaymentRequestResult>> RequestPaymentAsync(PaymentRequest request);
        Task<Result<PaymentVerificationResult>> VerifyPaymentAsync(PaymentVerificationRequest request);
    }
}
