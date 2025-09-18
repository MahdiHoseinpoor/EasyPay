using EasyPay.Shared.Models.Report;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{

    public interface IPaymentService
    {
        Task<GatewayDepositResponse?> RequestGatewayDeposit(GatewayDepositRequest request);
    }

    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GatewayDepositResponse?> RequestGatewayDeposit(GatewayDepositRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Payment.RequestGatewayDeposit, request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GatewayDepositResponse>();
            }
            return null;
        }
    }
}