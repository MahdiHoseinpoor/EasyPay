using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EasyPay.Web.Auth;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly JwtSecurityTokenHandler _jwtHandler = new();

    public ApiAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var tokenContent = _jwtHandler.ReadJwtToken(savedToken);
        if (tokenContent.ValidTo < DateTime.UtcNow)
        {
            await _localStorage.RemoveItemAsync("authToken");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var claimsPrincipal = CreateClaimsPrincipalFromToken(savedToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

        return new AuthenticationState(claimsPrincipal);
    }
    public void MarkUserAsAuthenticated(string token)
    {
        var authenticatedUser = CreateClaimsPrincipalFromToken(token);
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    private ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
    {
        var tokenContent = _jwtHandler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(tokenContent.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }
}