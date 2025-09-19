using EasyPay.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Identity;
using System.Net.Http.Json;

namespace EasyPay.Web.Services
{
    public interface IAuthItemsService
    {
        Task<List<AuthItemDto>> getAllAuthItems();
    }
    public class AuthItemsService(HttpClient httpClient) : IAuthItemsService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<AuthItemDto>> getAllAuthItems()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<Result<List<AuthItemDto>>>(ApiEndpoints.AuthItems.GetAll);
                if (result != null && result.IsSuccess)
                {
                    return result.Value ?? new List<AuthItemDto>();
                }
                return new List<AuthItemDto>();
            }
            catch
            {
                // Log exception
                return new List<AuthItemDto>();
            }
        }
    }
}
