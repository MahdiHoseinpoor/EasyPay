using EasyPay.Common;
using EasyPay.Shared.DTOs;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{

    public interface IProfileService
    {
        Task<List<AccountDto>> GetMyAccounts();
        Task<PagedList<TransactionDto>> GetTransactionHistory(Guid accountId, int page, int pageSize);
    }
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AccountDto>> GetMyAccounts()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AccountDto>>>(ApiEndpoints.Profile.MyAccounts);
                return result.IsSuccess ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedList<TransactionDto>> GetTransactionHistory(Guid accountId, int page, int pageSize)
        {
            var url = $"{ApiEndpoints.Profile.MyTransactionHistory(accountId)}?pageIndex={page}&pageSize={pageSize}";
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<PagedList<TransactionDto>>>(url);
                return result.IsSuccess ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }
    }
}