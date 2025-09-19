namespace EasyPay.Common.Errors.Business
{
    public record ResourceConflictError(string message) : Error(409, message)
    {
    }
}