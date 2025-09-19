namespace EasyPay.Common.Errors.Business
{
    public record InvalidVerificationCodeError() : Error(400, "Invalid or expired verification code.")
    {
    }
}