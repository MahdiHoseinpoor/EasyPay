namespace EasyPay.Common.Errors.Business
{
    public record InsufficientFundsError(decimal currentBalance, decimal withdrawalAmount)
        : Error(400, $"Insufficient funds for this withdrawal. Current balance is {currentBalance}, but {withdrawalAmount} was requested.")
    {
    }
}