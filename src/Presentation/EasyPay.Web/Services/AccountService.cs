using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.Models.AccountManagement;
using System.Net.Http.Json;

namespace EasyPay.Web.Services
{
    public interface IAccountService
    {
        Task<List<AccountTypeDto>> GetAccountTypes();
        Task<bool> CreateAccount(CreateAccountRequest command);
    }
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AccountTypeDto>> GetAccountTypes()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AccountTypeDto>>>(ApiEndpoints.AccountTypes.GetAll);
                return result.IsSuccess ? result.Value : new List<AccountTypeDto>();
            }
            catch
            {
                // Log exception
                return new List<AccountTypeDto>();
            }
        }

        public async Task<bool> CreateAccount(CreateAccountRequest command)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Accounts.Create, command);
            return response.IsSuccessStatusCode;
        }
    }
}
