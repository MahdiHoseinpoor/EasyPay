using EasyPay.Shared.Models.Report;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EasyPay.Web.Services
{
    public interface ITransactionService
    {
        Task<bool> TransferMoney(TransferMoneyRequest command);
        Task<bool> WithdrawToBankCard(WithdrawToBankCardRequest request);
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

        public async Task<bool> WithdrawToBankCard(WithdrawToBankCardRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Transaction.WithdrawToBankCard, request);
            return response.IsSuccessStatusCode;
        }
    }
}