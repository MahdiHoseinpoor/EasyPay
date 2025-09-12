using EasyPay.Api.Services;
using EasyPay.Application.Behaviors;
using EasyPay.Application.Services;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using System.Reflection;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(_ => { },Assembly.GetExecutingAssembly());
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection(StorageSettings.SectionName));
            builder.Services.AddScoped<IAccountNumberService, AccountNumberService>();
            builder.Services.AddScoped<IBillInquiryService, FakeBillInquiryService>();
            builder.Services.AddScoped<ISmsService, FakeSmsService>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();
            builder.Services.AddSingleton<IVerificationCodeCacheService, VerificationCodeCacheService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            });
        }
    }
}
