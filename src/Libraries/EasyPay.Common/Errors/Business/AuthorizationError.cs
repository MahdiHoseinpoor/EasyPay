namespace EasyPay.Common.Errors.Business
{
    public record AuthorizationError(string message = "Forbidden: You do not have permission to access this resource.") : Error(403, message)
    {
    }
}