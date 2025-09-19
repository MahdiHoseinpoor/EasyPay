using Blazored.LocalStorage;
using EasyPay.Shared.Models.Identity;
using EasyPay.Web.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EasyPay.Web.Services
{
    public interface IAuthService
    {
        Task<bool> RequestPhoneVerification(RegisterPhoneRequest request);
        Task<AuthenticateWithPhoneResponse> VerifyPhone(AuthenticateWithPhoneRequest request);
        Task<VerifyPasswordResponse> VerifyPassword(VerifyPasswordRequest request);
        Task<bool> CompleteProfile(CompleteProfileRequest request);
        Task Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }    

        public async Task<bool> CompleteProfile(CompleteProfileRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Identity.RegisterCompleteProfile, request);
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> RequestPhoneVerification(RegisterPhoneRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Identity.RegisterRequestCode, request);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthenticateWithPhoneResponse> VerifyPhone(AuthenticateWithPhoneRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Identity.RegisterVerifyPhone, request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var verifyResponse = await response.Content.ReadFromJsonAsync<AuthenticateWithPhoneResponse>();

            if (verifyResponse != null && !string.IsNullOrWhiteSpace(verifyResponse.Token))
            {
                await _localStorage.SetItemAsync("authToken", verifyResponse.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(verifyResponse.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", verifyResponse.Token);
            }

            return verifyResponse;
        }

        public async Task<VerifyPasswordResponse> VerifyPassword(VerifyPasswordRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Identity.VerifyPassword, request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var verifyResponse = await response.Content.ReadFromJsonAsync<VerifyPasswordResponse>();

            if (verifyResponse != null && !string.IsNullOrWhiteSpace(verifyResponse.Token))
            {
                await _localStorage.SetItemAsync("authToken", verifyResponse.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(verifyResponse.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", verifyResponse.Token);
            }

            return verifyResponse;
        }
    }
}