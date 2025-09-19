using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.Models.AccountManagement;
using System.Net.Http.Json;

namespace EasyPay.Web.Services
{
    public interface IAccountService
    {
        Task<List<AccountTypeDto>> GetAccountTypes();
        Task<List<AccountTypeDocumentRequirementDto>> GetRequirementsForAccountType(int accountTypeId);
        Task<bool> CreateAccount(CreateAccountRequest command);
        Task<AccountDto?> GetAccountById(Guid accountId);
        Task<AccountHolderDto?> InquireAccountHolderAsync(string accountNumber);
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
                var result = await _httpClient.GetFromJsonAsync<Result<List<AccountTypeDto>>>(ApiEndpoints.AccountTypes.GetAll);
                return result.IsSuccess ? result.Value : new List<AccountTypeDto>();
        }
        public async Task<List<AccountTypeDocumentRequirementDto>> GetRequirementsForAccountType(int accountTypeId)
        {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AccountTypeDocumentRequirementDto>>>(ApiEndpoints.AccountTypes.GetRequirements(accountTypeId));
                return result is { IsSuccess: true } ? result.Value ?? new List<AccountTypeDocumentRequirementDto>() : new List<AccountTypeDocumentRequirementDto>();
        }


        public async Task<bool> CreateAccount(CreateAccountRequest command)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Accounts.Create, command);
            return response.IsSuccessStatusCode;
        }

        public async Task<AccountDto?> GetAccountById(Guid accountId)
        {
          var result = await _httpClient.GetFromJsonAsync<AccountDto>(ApiEndpoints.Accounts.GetById(accountId));
          return result;
        }

        public async Task<AccountHolderDto?> InquireAccountHolderAsync(string accountNumber)
        {
            var result = await _httpClient.GetFromJsonAsync<AccountHolderDto>(ApiEndpoints.Accounts.Inquire(accountNumber));
            return result;
        }
    }
}
