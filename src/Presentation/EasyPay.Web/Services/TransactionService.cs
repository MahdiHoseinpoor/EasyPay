using EasyPay.Shared.Models.Report;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{
    public interface ITransactionService
    {
        Task<bool> TransferMoney(TransferMoneyRequest command);
    }
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        public TransactionService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<bool> TransferMoney(TransferMoneyRequest command)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Transaction.Transfer, command);
            return response.IsSuccessStatusCode;
        }
    }
}