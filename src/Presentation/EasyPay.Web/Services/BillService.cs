using EasyPay.Common;
using EasyPay.Shared.Models.Payment;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace EasyPay.Web.Services
{
    public interface IBillService
    {
        Task<Guid?> PayMobileBill(CreateMobileBillRequest command);
    }
    public class BillService : IBillService
    {
        private readonly HttpClient _httpClient;
        public BillService(HttpClient httpClient) { _httpClient = httpClient; }
        public async Task<Guid?> PayMobileBill(CreateMobileBillRequest command)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Bills.PayMobileBill, command);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
                return result.Value;
            }
            return null;
        }
    }
}