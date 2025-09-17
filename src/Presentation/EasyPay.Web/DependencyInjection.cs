
using EasyPay.Web;
using EasyPay.Web.Auth; 
using EasyPay.Web.DelegatingHandlers;
using EasyPay.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddTransient<ErrorHandlingDelegatingHandler>();

            services.AddHttpClient(Const.DefaultClientName, client =>
            {
                //string baseAddress = configuration["ApiSettings:BaseUrl"]
                //    ?? throw new InvalidOperationException("API BaseUrl is not configured in appsettings.json");

                //client.BaseAddress = new Uri(baseAddress);
                client.BaseAddress = new Uri("http://localhost:5144");
            })
            .AddHttpMessageHandler<ErrorHandlingDelegatingHandler>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpClient<IAuthService, AuthService>(Const.DefaultClientName);
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddHttpClient<AuthenticationStateProvider,ApiAuthenticationStateProvider>(Const.DefaultClientName);
            services.AddHttpClient<IAccountService, AccountService>(Const.DefaultClientName);
            services.AddHttpClient<IBankCardService, BankCardService>(Const.DefaultClientName);
            services.AddHttpClient<IBillService, BillService>(Const.DefaultClientName);
            services.AddHttpClient<IProfileService, ProfileService>(Const.DefaultClientName);
            services.AddHttpClient<ITransactionService, TransactionService>(Const.DefaultClientName);
            services.AddHttpClient<IAuthItemsService, AuthItemsService>(Const.DefaultClientName);

            return services;
        }
    }
}