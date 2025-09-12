namespace EasyPay.Common.Errors.Business
{
    public record ResourceInUseError(string message) : Error(400, message)
    {
    }
}