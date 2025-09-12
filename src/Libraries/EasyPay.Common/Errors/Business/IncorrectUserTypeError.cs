namespace EasyPay.Common.Errors.Business
{
    public record IncorrectUserTypeError(string message) : Error(400, message)
    {
    }
}