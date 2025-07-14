using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddRepositoriesFormAssembly(this IServiceCollection services,Assembly assembly)
        {
            var classRepositories = assembly.GetTypes().Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Repository"));
            var interfaceRepositories = assembly.GetTypes().Where(p => p.IsInterface && p.Name.EndsWith("Repository"));
            foreach (var classRepository in classRepositories)
            {
                var interfaceRepository = interfaceRepositories.FirstOrDefault(p => p.Name == "I" + classRepository.Name);
                if(interfaceRepository != null)
                {
                    services.AddScoped(interfaceRepository, classRepository);
                }
            }
        }
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddRepositoriesFormAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddIdentityCore<ApplicationUser>(
                options => {
            
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
