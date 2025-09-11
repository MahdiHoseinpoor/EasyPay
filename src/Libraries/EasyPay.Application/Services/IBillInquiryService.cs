using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public interface IBillInquiryService
    {
        /// <summary>
        /// Inquires about a mobile phone bill.
        /// </summary>
        /// <param name="phoneNumber">The mobile number to inquire about.</param>
        /// <returns>A result containing the bill details.</returns>
        Task<Result<BillDetails>> GetMobileBillAsync(string phoneNumber);

        // In the future, you could add:
        // Task<Result<BillDetails>> GetUtilityBillAsync(string billNumber);
    }

    public record BillDetails(decimal Amount, string BillType, string BillNumber);
}