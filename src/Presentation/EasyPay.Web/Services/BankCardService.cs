using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.Models.AccountManagement;
using EasyPay.Web.Pages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{
    public interface IBankCardService
    {
        Task<List<BankCardDto>> GetMyCardsAsync();
        Task<BankCardDto> GetCardByIdAsync(int id);
        Task<bool> CreateCardAsync(CreateBankCardRequest model);
        //Task<bool> UpdateCardAsync(int id, BankCardModel model);
        Task<bool> DeleteCardAsync(int id);
    }

    public class BankCardService : IBankCardService
    {
        private readonly HttpClient _httpClient;
        public BankCardService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<BankCardDto>> GetMyCardsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<Result<List<BankCardDto>>>(ApiEndpoints.Profile.MyBankCards);
            return result?.IsSuccess == true ? result.Value : new List<BankCardDto>();
        }

        public async Task<BankCardDto> GetCardByIdAsync(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<Result<BankCardDto>>(ApiEndpoints.BankCards.GetById(id));
            return result?.IsSuccess == true ? result.Value : null;
        }

        public async Task<bool> CreateCardAsync(CreateBankCardRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.BankCards.Create, model);
            return response.IsSuccessStatusCode;
        }

       

        public async Task<bool> DeleteCardAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(ApiEndpoints.BankCards.Delete(id));
            return response.IsSuccessStatusCode;
        }
    }
}