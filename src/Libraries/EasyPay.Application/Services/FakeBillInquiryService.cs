using EasyPay.Application.Services;
using EasyPay.Common;
using System;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public class FakeBillInquiryService : IBillInquiryService
    {
        public async Task<Result<BillDetails>> GetMobileBillAsync(string phoneNumber)
        {
            await Task.Delay(200);

            if (phoneNumber.EndsWith("00"))
            {
                return Result<BillDetails>.Failure(new NotFoundError("No active bill found for this phone number."));
            }

            var random = new Random();
            var amount = (decimal)random.Next(5000, 75000) / 100;

            var billDetails = new BillDetails(amount, "Mobile", phoneNumber);

            return Result<BillDetails>.Success(billDetails);
        }
    }
}