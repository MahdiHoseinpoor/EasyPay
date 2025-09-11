using Microsoft.Extensions.Hosting;
using System.Reflection;
using FluentValidation;
using EasyPay.Application.Services;
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
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
