using Blazored.LocalStorage;
using EasyPay.Web;
using EasyPay.Web.Auth;
using EasyPay.Web.DelegatingHandlers;
using EasyPay.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddScoped(sp =>
{
    var inner = new HttpClientHandler();
    var errorHandling = new ErrorHandlingDelegatingHandler { InnerHandler = inner };
    return new HttpClient(errorHandling) { BaseAddress = new Uri("http://localhost:5144") };
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthItemsService, AuthItemsService>();
builder.Services.AddScoped<IBankCardService, BankCardService>();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddSingleton<AuthState>();
var app = builder.Build();
await app.RunAsync();
