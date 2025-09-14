using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Identity;
using EasyPay.Shared.DTOs.Report;
using Microsoft.Extensions.Logging;
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
        Task<IPagedList<TransactionDto>> GetTransactionHistory(Guid accountId, int page, int pageSize);
        Task<List<AuthItemValueDto>> GetMySubmittedDocuments();
    }

    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(HttpClient httpClient, ILogger<ProfileService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AccountDto>> GetMyAccounts()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AccountDto>>>(ApiEndpoints.Profile.MyAccounts);
                if (result != null && result.IsSuccess)
                {
                    return result.Value ?? new List<AccountDto>();
                }
                if (result != null)
                {
                    _logger.LogWarning("API call to get accounts failed with message: {ErrorMessage}", result.error.message);
                }

                return new List<AccountDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching user accounts.");
                return new List<AccountDto>();
            }
        }

        public async Task<IPagedList<TransactionDto>> GetTransactionHistory(Guid accountId, int page, int pageSize)
        {
            var url = $"{ApiEndpoints.Profile.MyTransactionHistory(accountId)}?pageIndex={page}&pageSize={pageSize}";
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<IPagedList<TransactionDto>>>(url);
                if (result != null && result.IsSuccess)
                {
                    return result.Value;
                }

                if (result != null)
                {
                    _logger.LogWarning("API call to get transaction history failed: {ErrorMessage}", result.error.message);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching transaction history for account {AccountId}", accountId);
                return null;
            }
        }
        public async Task<List<AuthItemValueDto>> GetMySubmittedDocuments()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AuthItemValueDto>>>(ApiEndpoints.Profile.MySubmittedDocuments);
                return result is { IsSuccess: true } ? result.Value ?? new List<AuthItemValueDto>() : new List<AuthItemValueDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching user's submitted documents.");
                return new List<AuthItemValueDto>();
            }
        }
    }
}