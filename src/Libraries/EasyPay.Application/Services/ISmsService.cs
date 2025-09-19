using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    /// <summary>
    /// Defines a contract for sending SMS messages.
    /// The implementation will reside in the Infrastructure layer.
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends a verification code to a specified phone number.
        /// </summary>
        /// <param name="phoneNumber">The recipient's phone number.</param>
        /// <param name="code">The verification code to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendVerificationCodeAsync(string phoneNumber, string code);
    }
}