using EasyPay.Common;
using EasyPay.Common.Errors.Business;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyPay.Application.Services.PaymentGateways
{
    public class ZarinpalGatewayService : IPaymentGatewayService
    {
        private readonly HttpClient _httpClient;
        private readonly ZarinpalSettings _settings;
        private readonly ILogger<ZarinpalGatewayService> _logger;
        private readonly string _requestUrl;
        private readonly string _verifyUrl;
        private readonly string _startPayUrl;

        public string GatewayName => "Zarinpal";

        public ZarinpalGatewayService(HttpClient httpClient, IOptions<ZarinpalSettings> settings, ILogger<ZarinpalGatewayService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            var baseUrl = _settings.IsSandbox
                ? "https://sandbox.zarinpal.com"
                : "https://api.zarinpal.com";

            _requestUrl = $"{baseUrl}/pg/v4/payment/request.json";
            _verifyUrl = $"{baseUrl}/pg/v4/payment/verify.json";
            _startPayUrl = $"{baseUrl}/pg/StartPay/";
        }

        public async Task<Result<PaymentRequestResult>> RequestPaymentAsync(PaymentRequest request)
        {
            if (string.IsNullOrEmpty(_settings.MerchantId) || _settings.MerchantId.Length != 36)
            {
                _logger.LogError("Zarinpal MerchantId is not configured correctly. Please check appsettings.json.");
                return Result<PaymentRequestResult>.Failure(new BusinessRuleError("Payment gateway is not configured."));
            }

            var apiRequest = new
            {
                merchant_id = _settings.MerchantId,
                amount = (long)request.Amount,
                currency = request.Currency,
                description = request.Description,
                callback_url = request.CallbackUrl,
                metadata = new { order_id = request.TransactionId }
            };

            _logger.LogInformation("Sending payment request to Zarinpal for TransactionId: {TransactionId}", request.TransactionId);

            var response = await _httpClient.PostAsJsonAsync(_requestUrl, apiRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Zarinpal payment request failed. StatusCode: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);
                return Result<PaymentRequestResult>.Failure(new BusinessRuleError("Failed to connect to payment gateway."));
            }

            try
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ZarinpalRequestResponse>();

                if (apiResponse?.Data is not null && apiResponse.Data.Code == 100)
                {
                    var result = new PaymentRequestResult(
                        RedirectUrl: $"{_startPayUrl}{apiResponse.Data.Authority}",
                        Token: apiResponse.Data.Authority
                    );
                    _logger.LogInformation("Zarinpal payment request successful. Authority: {Authority}", result.Token);
                    return Result<PaymentRequestResult>.Success(result);
                }

                var errorData = apiResponse?.GetErrorObject();
                var errorCode = errorData?.Code ?? -1;
                var errorMessage = errorData?.Message ?? "Unknown Zarinpal error.";

                _logger.LogWarning("Zarinpal returned a business error. Code: {ErrorCode}, Message: {ErrorMessage}", errorCode, errorMessage);
                return Result<PaymentRequestResult>.Failure(new BusinessRuleError($"Gateway error: {errorMessage} (Code: {errorCode})"));
            }
            catch (JsonException ex)
            {
                var rawContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(ex, "Failed to deserialize Zarinpal response. Raw content: {RawContent}", rawContent);
                return Result<PaymentRequestResult>.Failure(new BusinessRuleError("Received an invalid response from the payment gateway."));
            }
        }

        public async Task<Result<PaymentVerificationResult>> VerifyPaymentAsync(PaymentVerificationRequest request)
        {
            var apiRequest = new
            {
                merchant_id = _settings.MerchantId,
                amount = (long)request.Amount,
                authority = request.Token
            };

            _logger.LogInformation("Sending payment verification to Zarinpal for Authority: {Authority}", request.Token);

            var response = await _httpClient.PostAsJsonAsync(_verifyUrl, apiRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Zarinpal payment verification failed. StatusCode: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);
                return Result<PaymentVerificationResult>.Failure(new BusinessRuleError("Failed to verify payment with gateway."));
            }

            try
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ZarinpalVerifyResponse>();

                if (apiResponse?.Data is not null && (apiResponse.Data.Code == 100 || apiResponse.Data.Code == 101))
                {
                    var result = new PaymentVerificationResult(FinalReferenceId: apiResponse.Data.RefId.ToString());
                    _logger.LogInformation("Zarinpal payment verification successful. RefId: {RefId}", result.FinalReferenceId);
                    return Result<PaymentVerificationResult>.Success(result);
                }

                var errorData = apiResponse?.GetErrorObject();
                var errorCode = errorData?.Code ?? -1;
                var errorMessage = errorData?.Message ?? "Unknown Zarinpal error.";
                _logger.LogWarning("Zarinpal verification returned a business error. Code: {ErrorCode}, Message: {ErrorMessage}", errorCode, errorMessage);
                return Result<PaymentVerificationResult>.Failure(new BusinessRuleError($"Gateway verification failed: {errorMessage} (Code: {errorCode})"));
            }
            catch (JsonException ex)
            {
                var rawContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(ex, "Failed to deserialize Zarinpal verification response. Raw content: {RawContent}", rawContent);
                return Result<PaymentVerificationResult>.Failure(new BusinessRuleError("Received an invalid verification response from the payment gateway."));
            }
        }

        // --- CORRECTED AND ROBUST DTOs ---

        private class ZarinpalRequestResponse
        {
            [JsonPropertyName("data")]
            public ZarinpalRequestData? Data { get; init; }

            [JsonPropertyName("errors")]
            public JsonElement Errors { get; init; } // The property bound to the JSON

            // A method to safely get the error object, avoiding property name collision
            public ZarinpalError? GetErrorObject() =>
                Errors.ValueKind == JsonValueKind.Object
                ? Errors.Deserialize<ZarinpalError>()
                : null;
        }

        private class ZarinpalVerifyResponse
        {
            [JsonPropertyName("data")]
            public ZarinpalVerifyData? Data { get; init; }

            [JsonPropertyName("errors")]
            public JsonElement Errors { get; init; }

            public ZarinpalError? GetErrorObject() =>
                Errors.ValueKind == JsonValueKind.Object
                ? Errors.Deserialize<ZarinpalError>()
                : null;
        }

        private record ZarinpalRequestData(int Code, string Message, string Authority, string FeeType, long Fee);
        private record ZarinpalVerifyData(int Code, string Message, string CardHash, string CardPan, long RefId, string FeeType, long Fee);
        private record ZarinpalError(int Code, string Message);
    }
}