using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public interface IAccountNumberService
    {
        Task<string> GenerateUniqueAccountNumberAsync();
    }
}