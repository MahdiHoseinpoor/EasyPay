namespace EasyPay.Common.Errors.Business
{
    public record AccountInactiveError(string accountStatus) : Error(400, $"Account is not active. Current status: {accountStatus}")
    {
    }
}